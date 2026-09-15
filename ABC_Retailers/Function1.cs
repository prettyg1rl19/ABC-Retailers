using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues.Models;
using Google.Protobuf.Compiler;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ABC_Retailers;

public class Function1
{
    private readonly ILogger<Function1> _logger;
    private readonly string _storageConnectionString;
    private TableClient _tableClient;
    private BlobContainerClient _blobContainerClient;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
        //Read connection string from enviroment
        //(local.settings.json or Azure App settings)
        _storageConnectionString = Environment.GetEnvironmentVariable("connection");
        //Create table client
        var serviceClient = new TableServiceClient(_storageConnectionString);
        _tableClient = serviceClient.GetTableClient("customer");
        //initialize blob continner client for "profile-pics" container
        _blobContainerClient = new BlobContainerClient(_storageConnectionString, "product");
        _blobContainerClient.CreateIfNotExists(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
    }

    //This is for adding the customer to the table!
    [Function(nameof(Function1))]
    public async Task Run([QueueTrigger("abc-retailers", Connection = "connection")] QueueMessage message)
    {
        _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);

        //Create the table if it doesn't exsist
        await _tableClient.CreateIfNotExistsAsync();

        //1. Manuually deserialize the JSON string into our project
        var customer = JsonSerializer.Deserialize<CustomerEntity>(message.MessageText);

        if (customer == null)
        {
            _logger.LogError("Failed to deserialize JSON message");
            return;
        }

        //2. CRITICAL STEP: Set the required PartitionKey and RowKey
        customer.RowKey = Guid.NewGuid().ToString();
        customer.PartitionKey = "Customer";

        _logger.LogInformation($"Saving entity with RowKey: {customer.RowKey}");

        //3. Manually add the entity to the table
        await _tableClient.AddEntityAsync(customer);
        _logger.LogInformation("Sucessfully saved the customer to the table!");
    }

    [Function("GetCustomers")]
    public async Task<HttpResponseData> GetCustomers([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request to get all customers.");

        try
        {
            // Maually query the table. This returns an async collection of all entities.
            var customer = await _tableClient.QueryAsync<CustomerEntity>().ToListAsync();
            // Create an OR (200) response and write the list of people as JSON
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(customer);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query table storage");
            //Create an error response
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync("An error occured whole retrieving data from the table");
            return response;
        }
    }

    // Blob Storage Upload

    [Function("AddProduct")]
    public async Task<HttpResponseData> AddProduct([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "product")] HttpRequestData req)
    {
        _logger.LogInformation("C# HTTP trigger function to add aproduct with an image received a request.");

        var newProduct = new ProductEntity();
        string? uploadedBlobUrl = null;

        // 1. Parse the multipart from data
        var multipartReader = new MultipartReader(req.Headers.GetValues("Content-Type").First().Split(';')[1].Trim().Split('=')[1], req.Body);
        var section = await multipartReader.ReadNextSectionAsync();

        while (section != null)
        {
            var contentDisposition = section.Headers["Content-Disposition"].ToString();
            var productName = contentDisposition.Split(';')[1].Trim().Split('=')[1].Trim('"');

            if (productName == "Name" || productName == "Description" || productName == "Price")
            {
                var value = await new StreamReader(section.Body).ReadToEndAsync();
                if (productName == "Name") newProduct.Product_Name = value;
                if (productName == "Description") newProduct.Description = value;
                if (productName == "Price") newProduct.Price = value;
            }
            else if (productName == "ProfileImage")
            {
                var fileName = contentDisposition.Split(';')[2].Trim().Split('=')[1].Trim('"');
                var uniqueFileName = $"{Guid.NewGuid()}-{Path.GetFileName(fileName)}{Path.GetExtension(fileName)}";
                var blobClient = _blobContainerClient.GetBlobClient(uniqueFileName);

                // 2. Upload the file stream to Blob Storage
                await blobClient.UploadAsync(section.Body, true);
                uploadedBlobUrl = blobClient.Uri.ToString();
            }
            section = await multipartReader.ReadNextSectionAsync();
        }

        // 3. Validate and save to Table Storage
        if (string.IsNullOrEmpty(newProduct.Product_Name) || string.IsNullOrEmpty(newProduct.Description) || string.IsNullOrEmpty(newProduct.Price) || string.IsNullOrEmpty(uploadedBlobUrl))
        {
            return req.CreateResponse(HttpStatusCode.NotFound);
        }

        newProduct.PartitionKey = "Product";
        newProduct.RowKey = Guid.NewGuid().ToString();
        newProduct.ImageUrl = uploadedBlobUrl;

        await _tableClient.AddEntityAsync(newProduct);
        _logger.LogInformation($"Successfully added {newProduct.Product_Name} with a product image.");

        return req.CreateResponse(HttpStatusCode.Created);
    }
}