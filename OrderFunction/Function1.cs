using System;
using System.Net;
using System.Text.Json;
using Azure.Data.Tables;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace OrderFunction;

public class Function1
{
    private readonly ILogger<Function1> _logger;
    private readonly string _storageConnectionString;
    private TableClient _tableClient;
    private readonly TableServiceClient _serviceClient;


    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
        //Read connection string from enviroment
        //(local.settings.json or Azure App settings)
        _storageConnectionString = Environment.GetEnvironmentVariable("connection");
        //Create table client
        var serviceClient = new TableServiceClient(_storageConnectionString);
        _tableClient = serviceClient.GetTableClient("order");

        var connectionString = Environment.GetEnvironmentVariable("connection");
        _serviceClient = new TableServiceClient(connectionString);


    }

    [Function(nameof(Function1))]
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
            await tableClient.CreateIfNotExistsAsync();
            var orders = await tableClient.QueryAsync<OrderEntity>(o => o.PartitionKey == "OrderPartition").ToListAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(orders);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync("Error retrieving orders");
            return response;
        }
    }


    [Function("EditOrder")]
    public async Task<HttpResponseData> EditOrder(
    [HttpTrigger(AuthorizationLevel.Function, "put", Route = "order/{partitionKey}/{rowKey}")] HttpRequestData req, string partitionKey, string rowKey)
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
}