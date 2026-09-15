using Azure.Storage.Queues;
using System.Text.Json;

namespace TestQueue
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Your real Azure Storage connection string
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=st10438767cldv;AccountKey=y6ivMA3KaV9FoXiBcjRgioufhXVKwNsAedeE4uMdhD+XH4XCwWEz2PunwZPRLkHWOFnldaa47XDo+AStxxQzCw==;EndpointSuffix=core.windows.net";

            // Queue name must match the one your function listens to
            var queueClient = new QueueClient(
                connectionString,
                "customer-trigger",
                new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 } // ensure its base64
            );

            // Create queue if it doesn't exist
            await queueClient.CreateIfNotExistsAsync();

            // Build test object
            var customer = new { Customer_Name = "Clark Kent", Customer_Email = "supernerd@gmail.com", Customer_Address = "Metropolis" };

            // Serialize object to JSON
            string json = JsonSerializer.Serialize(customer);

            // Send as Plain JSON string
            await queueClient.SendMessageAsync(json);

            Console.WriteLine($"Message sent: {json}");
        }
    }
}
