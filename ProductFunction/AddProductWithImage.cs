using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ProductFunction
{
    public class AddProductWithImage
    {
        private readonly ILogger<AddProductWithImage> _logger;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly TableClient _tableClient;

        public AddProductWithImage(ILogger<AddProductWithImage> logger)
        {
            _logger = logger;
            var connectionString = Environment.GetEnvironmentVariable("connection");
            _blobContainerClient = new BlobContainerClient(connectionString, "product");
            _blobContainerClient.CreateIfNotExists();
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient("product");
            _tableClient.CreateIfNotExists();
        }

        [Function("AddProductWithImage")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")] HttpRequestData req)
        {
            _logger.LogInformation("Processing request to add product with image.");
            var newProduct = new ProductEntity();
            string? uploadedBlobUrl = null;

            var boundary = req.Headers.GetValues("Content-Type").First().Split(';')[1].Trim().Split('=')[1];
            var reader = new MultipartReader(boundary, req.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var contentDisposition = section.Headers["Content-Disposition"].ToString();
                var name = contentDisposition.Split(';')[1].Trim().Split('=')[1].Trim('"');

                if (name == "productName" || name == "productDescription" || name == "productPrice")
                {
                    var value = await new StreamReader(section.Body).ReadToEndAsync();
                    if (name == "productName") newProduct.Product_Name = value;
                    if (name == "productDescription") newProduct.Description = value;
                    if (name == "productPrice") newProduct.Price = int.Parse(value);
                }
                else if (name == "file")
                {
                    var fileName = contentDisposition.Split(';')[2].Trim().Split('=')[1].Trim('"');
                    var uniqueFileName = $"{Guid.NewGuid()}-{Path.GetFileName(fileName)}";
                    var blobClient = _blobContainerClient.GetBlobClient(uniqueFileName);
                    await blobClient.UploadAsync(section.Body, true);
                    uploadedBlobUrl = blobClient.Uri.ToString();
                }

                section = await reader.ReadNextSectionAsync();
            }

            if (string.IsNullOrEmpty(newProduct.Product_Name) || string.IsNullOrEmpty(uploadedBlobUrl))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            newProduct.ImageUrl = uploadedBlobUrl;
            newProduct.PartitionKey = "ProductPartition";
            newProduct.RowKey = Guid.NewGuid().ToString();

            await _tableClient.AddEntityAsync(newProduct);
            _logger.LogInformation($"Product named '{newProduct.Product_Name}' was successfully added with an image.");

            return req.CreateResponse(HttpStatusCode.Created);
        }


    }
}
