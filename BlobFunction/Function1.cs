//using System;
//using System.Net;
//using System.Text.Json;
//using Azure.Data.Tables;
//using Azure.Storage.Blobs;
//using Azure.Storage.Queues.Models;
//using Microsoft.AspNetCore.WebUtilities;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Http;
//using Microsoft.Extensions.Logging;

//namespace BlobFunction;

//public class Function1
//{
//    private readonly ILogger<Function1> _logger;
//    private readonly string _storageConnectionString;
//    private TableClient _tableClient;
//    private BlobContainerClient _blobContainerClient;


//    public Function1(ILogger<Function1> logger)
//    {
//        _logger = logger;
//        var serviceClient = new TableServiceClient(_storageConnectionString);
//        _tableClient = serviceClient.GetTableClient("PeopleTable");
//        //initialize blob continner client for "profile-pics" container
//        _blobContainerClient = new BlobContainerClient(_storageConnectionString, "products");
//        _blobContainerClient.CreateIfNotExists(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
//    }

//    // Blob Storage Upload

//    [Function("AddProductWithImage")]
//    public async Task<HttpResponseData> AddProductWithImage([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")] HttpRequestData req)
//    {
//        _logger.LogInformation("C# HTTP trigger function to add person with image received a request.");

//        var newProd = new ProductEntity();
//        string? uploadedBlobUrl = null;

//        // 1. Parse the multipart from data
//        var multipartReader = new MultipartReader(req.Headers.GetValues("Content-Type").First().Split(';')[1].Trim().Split('=')[1], req.Body);
//        var section = await multipartReader.ReadNextSectionAsync();

//        while (section != null)
//        {
//            var contentDisposition = section.Headers["Content-Disposition"].ToString();
//            var name = contentDisposition.Split(';')[1].Trim().Split('=')[1].Trim('"');

//            if (name == "Name" || name == "Description" || name == "Price")
//            {
//                var value = await new StreamReader(section.Body).ReadToEndAsync();
//                if (name == "Name") newProd.Product_Name = value;
//                if (name == "Description") newProd.Description = value;
//                if (name == "Price") newProd.Price = value;
//            }
//            else if (name == "ImageUrl")
//            {
//                var fileName = contentDisposition.Split(';')[2].Trim().Split('=')[1].Trim('"');
//                var uniqueFileName = $"{Guid.NewGuid()}-{Path.GetFileName(fileName)}{Path.GetExtension(fileName)}";
//                var blobClient = _blobContainerClient.GetBlobClient(uniqueFileName);

//                // 2. Upload the file stream to Blob Storage
//                await blobClient.UploadAsync(section.Body, true);
//                uploadedBlobUrl = blobClient.Uri.ToString();
//            }
//            section = await multipartReader.ReadNextSectionAsync();
//        }

//        // 3. Validate and save to Table Storage
//        if (string.IsNullOrEmpty(newProd.Product_Name) || string.IsNullOrEmpty(newProd.Description) || string.IsNullOrEmpty(newProd.Price) || string.IsNullOrEmpty(uploadedBlobUrl))
//        {
//            return req.CreateResponse(HttpStatusCode.NotFound);
//        }

//        newProd.PartitionKey = "People";
//        newProd.RowKey = Guid.NewGuid().ToString();
//        newProd.ImageUrl = uploadedBlobUrl;

//        await _tableClient.AddEntityAsync(newProd);
//        _logger.LogInformation($"Successfully added {newProd.Product_Name} and uploaded their profile picture.");

//        return req.CreateResponse(HttpStatusCode.Created);
//    }

//    [Function("GetProducts")]
//    public async Task<HttpResponseData> GetProducts([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "product")] HttpRequestData req)
//    {
//        _logger.LogInformation("C# HTTP trigger function processed a rewuest to get all people.");

//        try
//        {
//            // Maually query the table. This returns an async collection of all entities.
//            var products = await _tableClient.QueryAsync<ProductEntity>().ToListAsync();
//            // Create an OR (200) response and write the list of people as JSON
//            var response = req.CreateResponse(HttpStatusCode.OK);
//            await response.WriteAsJsonAsync(products);
//            return response;
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Failed to query table storage");
//            //Create an error response
//            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
//            await response.WriteStringAsync("An error occured whole retrieving data from the table");
//            return response;
//        }
//    }
//}