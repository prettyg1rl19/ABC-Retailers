using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace POEFunctions
{
    internal class TableFunction
    {
        private readonly ILogger<TableFunction> _logger;
        private readonly string _storageConnectionString;
        private TableClient _tableClient;
        private readonly TableServiceClient _serviceClient;

        public TableFunction(ILogger<TableFunction> logger)
        {
            _logger = logger;
            //Read connection string from enviroment
            //(local.settings.json or Azure App settings)
            _storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=st10438767cldv;AccountKey=y6ivMA3KaV9FoXiBcjRgioufhXVKwNsAedeE4uMdhD+XH4XCwWEz2PunwZPRLkHWOFnldaa47XDo+AStxxQzCw==;EndpointSuffix=core.windows.net";
            /*
             IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 5 Azure functions publish
             [video online] Available at:<https://www.youtube.com/watch?v=GXGN-aWbwO0&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=11> [Accessed 6 October 2025].
             */
            //Create table client
            var serviceClient = new TableServiceClient(_storageConnectionString);
            _tableClient = serviceClient.GetTableClient("customer");
        }

        [Function(nameof(TableFunction))]
        public async Task Run([QueueTrigger("customer-trigger", Connection = "connection")] QueueMessage message)
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

        [Function("GetCustomer")]
        public async Task<HttpResponseData> GetCustomers(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customer")] HttpRequestData req)
        {
            _logger.LogInformation("Fetching all customers from Table Storage.");

            try
            {
                var tableClient = _serviceClient.GetTableClient("customer");
                await tableClient.CreateIfNotExistsAsync();
                // Maually query the table. This returns an async collection of all entities.
                var customers = await tableClient.QueryAsync<CustomerEntity>().ToListAsync();

                // Create an OR (200) response and write the list of customers as JSON
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(customers);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to query customer table.");
                //Create an error response
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync("Error retrieving customers.");
                return response;
            }
        }
        /*
         IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 2 Azure functions and queues triggers
         [video online] Available at:<https://www.youtube.com/watch?v=zP4umzRCsTM&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=8> [Accessed 25 September 2025].
         */
    }
}

