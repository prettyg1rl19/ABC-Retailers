using System;
using System.Net;
using System.Text.Json;
using Azure.Data.Tables;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Table_Function;

namespace TableFunction;

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
        _tableClient = serviceClient.GetTableClient("customer");
    }

    [Function(nameof(Function1))]
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
            var customers = await tableClient.QueryAsync<CustomerEntity>().ToListAsync();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(customers);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to query customer table.");
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync("Error retrieving customers.");
            return response;
        }
    }
}