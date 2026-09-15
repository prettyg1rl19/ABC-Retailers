using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using POEFunctions.Entities;

namespace POEFunctions
{
    public class AzureFileFunction
    {
        private readonly ILogger<AzureFileFunction> _logger;
        private readonly ShareClient _shareClient;
        private readonly string _storageConnectionString;


        public AzureFileFunction(ILogger<AzureFileFunction> logger)
        {
            _logger = logger;
            _storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=st10438767cldv;AccountKey=y6ivMA3KaV9FoXiBcjRgioufhXVKwNsAedeE4uMdhD+XH4XCwWEz2PunwZPRLkHWOFnldaa47XDo+AStxxQzCw==;EndpointSuffix=core.windows.net";
            /*
             IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 5 Azure functions publish
             [video online] Available at:<https://www.youtube.com/watch?v=GXGN-aWbwO0&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=11> [Accessed 6 October 2025].
             */
            _shareClient = new ShareClient(_storageConnectionString, "fileshare");
            _shareClient.CreateIfNotExists();
        }

        [Function("ListFiles")]
        public async Task<HttpResponseData> ListFiles([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ListFiles")] HttpRequestData req)
        {
            _logger.LogInformation($"C# HTTP trigger function processed request to get all files.");
            var serviceClient = new ShareServiceClient(_storageConnectionString);
            var shareClient = serviceClient.GetShareClient("fileshare");

            var fileModels = new List<FileEntity>();
            try
            {
                var directoryClient = shareClient.GetDirectoryClient("uploads");
                await foreach (ShareFileItem item in directoryClient.GetFilesAndDirectoriesAsync())
                {
                    if (!item.IsDirectory)
                    {
                        var fileClient = directoryClient.GetFileClient(item.Name);
                        var properties = await fileClient.GetPropertiesAsync();
                        fileModels.Add(new FileEntity
                        {
                            Name = item.Name,
                            Size = properties.Value.ContentLength,
                            LastModified = properties.Value.LastModified
                        });
                    }
                }
            }
            catch (Exception ex) { throw new Exception("Error listing files: " + ex.Message, ex); }
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(fileModels);
            return response;
        }

        /*
         IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 2 Azure functions and queues triggers
         [video online] Available at:<https://www.youtube.com/watch?v=zP4umzRCsTM&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=8> [Accessed 25 September 2025].

         IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 3 Azure functions and MVC
         [video online] Available at:<https://www.youtube.com/watch?v=x7yTh85fQbw&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=9> [Accessed 26 September 2025].
         
         Mrzygłód, K., 2006. Azure for Developers. Birmingham: Packt Publishing Ltd.
         
        */


    }
}
