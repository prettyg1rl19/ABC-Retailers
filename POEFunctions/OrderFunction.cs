using Azure.Data.Tables;
using Azure.Storage.Queues.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using POEFunctions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace POEFunctions
{
    public class OrderFunction
    {
        //declare variables.
        private readonly ILogger<OrderFunction> _logger;
        private readonly string _storageConnectionString;
        private TableClient _tableClient;
        private readonly TableServiceClient _serviceClient;



        public OrderFunction(ILogger<OrderFunction> logger)
        {
            _logger = logger;
            //Read connection string from enviroment
            //(local.settings.json or Azure App settings)
            _storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=st10438767cldv;AccountKey=y6ivMA3KaV9FoXiBcjRgioufhXVKwNsAedeE4uMdhD+XH4XCwWEz2PunwZPRLkHWOFnldaa47XDo+AStxxQzCw==;EndpointSuffix=core.windows.net";
            //Create table client
            var serviceClient = new TableServiceClient(_storageConnectionString);
            _tableClient = serviceClient.GetTableClient("order");

            var connectionString = "DefaultEndpointsProtocol=https;AccountName=st10438767cldv;AccountKey=y6ivMA3KaV9FoXiBcjRgioufhXVKwNsAedeE4uMdhD+XH4XCwWEz2PunwZPRLkHWOFnldaa47XDo+AStxxQzCw==;EndpointSuffix=core.windows.net";
            _serviceClient = new TableServiceClient(connectionString);

            /*
             IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 5 Azure functions publish
             [video online] Available at:<https://www.youtube.com/watch?v=GXGN-aWbwO0&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=11> [Accessed 6 October 2025].
             */
        }

        [Function(nameof(OrderFunction))]
        public async Task Run([QueueTrigger("order-trigger", Connection = "connection")] QueueMessage message)
        {
            _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);
            //Create the table if it doesn't exsist
            await _tableClient.CreateIfNotExistsAsync();

            //1. Manuually deserialize the JSON string into our project
            var order = JsonSerializer.Deserialize<OrderEntity>(message.MessageText);

            if (order == null)
            {
                _logger.LogError("Failed to deserialize JSON message");
                return;
            }

            //2. CRITICAL STEP: Set the required PartitionKey and RowKey
            order.RowKey = Guid.NewGuid().ToString();
            order.PartitionKey = "OrderPartition";

            _logger.LogInformation($"Saving entity with RowKey: {order.RowKey}");

            //3. Manually add the entity to the table
            await _tableClient.AddEntityAsync(order);
            _logger.LogInformation("Sucessfully saved the order to the table!");
        }

        [Function("GetOrders")]
        public async Task<HttpResponseData> GetOrders(
         [HttpTrigger(AuthorizationLevel.Function, "get", Route = "order")] HttpRequestData req)
        {
            _logger.LogInformation("Fetching all orders from Table Storage.");

            try
            {
                var tableClient = _serviceClient.GetTableClient("order");

                // Create the table if it doesn't exsist
                await tableClient.CreateIfNotExistsAsync();
                var orders = await tableClient.QueryAsync<OrderEntity>(o => o.PartitionKey == "OrderPartition").ToListAsync();

                // Create an OR (200) response and write the list of orders as JSON
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(orders);
                return response;
            }
            catch (Exception ex)
            {
                //Create error response
                _logger.LogError(ex, "Error retrieving orders");
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync("Error retrieving orders");
                return response;
            }
        }


        [Function("EditOrder")]
        public async Task<HttpResponseData> EditOrder(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "order/{partitionKey}/{rowKey}")] HttpRequestData req, string partitionKey, string rowKey)
        //Mrzygłód, K., 2006. Azure for Developers. Birmingham: Packt Publishing Ltd.
        {
            _logger.LogInformation($"Processing a request to edit an order... Please stand by.");

            try
            {
                var tableClient = _serviceClient.GetTableClient("order");
                await tableClient.CreateIfNotExistsAsync();

                var requestBody = await req.ReadAsStringAsync();
                var updatedOrder = JsonSerializer.Deserialize<OrderEntity>(requestBody);

                if (updatedOrder == null)
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteStringAsync("Invalid order data.");
                    return badResponse;
                }

                updatedOrder.PartitionKey = partitionKey;
                updatedOrder.RowKey = rowKey;

                var existingOrderResponse = await tableClient.GetEntityAsync<OrderEntity>(partitionKey, rowKey);
                var existingOrder = existingOrderResponse.Value;

                if (updatedOrder.Order_Id == 0)
                {
                    updatedOrder.Order_Id = existingOrder.Order_Id;
                }

                await tableClient.UpdateEntityAsync(updatedOrder, existingOrder.ETag, TableUpdateMode.Replace);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteStringAsync("Order updated successfully.");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order, please try again.");
                var response = req.CreateResponse(HttpStatusCode.InternalServerError);
                await response.WriteStringAsync("Error updating order, please try again.");
                return response;
            }
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
