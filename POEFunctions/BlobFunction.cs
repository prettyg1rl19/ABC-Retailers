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
using POEFunctions.Entities;

namespace POEFunctions
{
    public class BlobFunction
    {
        private readonly ILogger<BlobFunction> _logger;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly TableClient _tableClient;

        public BlobFunction(ILogger<BlobFunction> logger)
        {
            _logger = logger;
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=st10438767cldv;AccountKey=y6ivMA3KaV9FoXiBcjRgioufhXVKwNsAedeE4uMdhD+XH4XCwWEz2PunwZPRLkHWOFnldaa47XDo+AStxxQzCw==;EndpointSuffix=core.windows.net";
            /*IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 5 Azure functions publish
              [video online] Available at:<https://www.youtube.com/watch?v=GXGN-aWbwO0&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=11> [Accessed 6 October 2025].*/
            _blobContainerClient = new BlobContainerClient(connectionString, "product");
            _blobContainerClient.CreateIfNotExists();
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient("product");
            _tableClient.CreateIfNotExists();
        }

        // Blob Storage Upload
        [Function("AddProductWithImage")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")] HttpRequestData req)
        {
            _logger.LogInformation("Processing request to add product with image.");
            var newProduct = new ProductEntity();
            string? uploadedBlobUrl = null;

            // 1. Parse the multipart from data
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

                    // 2. Upload the file stream to Blob Storage
                    await blobClient.UploadAsync(section.Body, true);
                    uploadedBlobUrl = blobClient.Uri.ToString();
                }

                section = await reader.ReadNextSectionAsync();
            }

            // 3. Validate and save to Table Storage
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

        /*
         IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 3 Azure functions and MVC
         [video online] Available at:<https://www.youtube.com/watch?v=x7yTh85fQbw&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=9> [Accessed 26 September 2025].

         IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 4 Azure functions and MVC and blobs
         [video online] Available at:<https://www.youtube.com/watch?v=r-VksPFfFpE&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=10> [Accessed 27 September 2025].
        */
    }
}