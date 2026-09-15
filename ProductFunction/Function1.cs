//using System.Net;
//using Azure.Data.Tables;
//using Azure.Storage.Blobs;
//using Azure.Storage.Queues.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.HttpLogging;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.WebUtilities;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Http;
//using Microsoft.Extensions.Logging;

//namespace ProductFunction;

//public class Function1
//{
//    private readonly ILogger<Function1> _logger;
//    private readonly string _storageConnectionString;
//    private TableClient _tableClient;
//    private BlobContainerClient _blobContainerClient;

//    public Function1(ILogger<Function1> logger)
//    {
//        _logger = logger;
//        //Read connection string from enviroment
//        //(local.settings.json or Azure App settings)
//        _storageConnectionString = Environment.GetEnvironmentVariable("connection");
//        //Create table client
//        var serviceClient = new TableServiceClient(_storageConnectionString);
//        _tableClient = serviceClient.GetTableClient("PeopleTable");
//        //initialize blob continner client for "profile-pics" container
//        _blobContainerClient = new BlobContainerClient(_storageConnectionString, "profile-pics");
//        _blobContainerClient.CreateIfNotExists(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
//    }

//    [Function(nameof(Function1))]
//    public async Task Run([HttpTrigger("people-trigger", Connection = "connection")] QueueMessage message)
//    {
//        _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);

//        //Create the table if it doesn't exsist
//        await _tableClient.CreateIfNotExistsAsync();

//        //1. Manuually deserialize the JSON string into our project
//        var person = JsonSerializer.Deserialize<PersonEntity>(message.MessageText);

//        if (person == null)
//        {
//            _logger.LogError("Failed to deserialize JSON message");
//            return;
//        }

//        //2. CRITICAL STEP: Set the required PartitionKey and RowKey
//        person.RowKey = Guid.NewGuid().ToString();
//        person.PartitionKey = "People";

//        _logger.LogInformation($"Saving entity with RowKey: {person.RowKey}");

//        //3. Manually add the entity to the table
//        await _tableClient.AddEntityAsync(person);
//        _logger.LogInformation("Sucessfullt saved the peron to the table");
//    }

//    [Function("GetPeople")]
//    public async Task<HttpResponseData> GetPeople([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "people")] HttpRequestData req)
//    {
//        _logger.LogInformation("C# HTTP trigger function processed a rewuest to get all people.");

//        try
//        {
//            // Maually query the table. This returns an async collection of all entities.
//            var people = await _tableClient.QueryAsync<PersonEntity>().ToListAsync();
//            // Create an OR (200) response and write the list of people as JSON
//            var response = req.CreateResponse(HttpStatusCode.OK);
//            await response.WriteAsJsonAsync(people);
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

//    // Blob Storage Upload

//    [Function("AddPersonWithImage")]
//    public async Task<HttpResponseData> AddPersonWithImage([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "people-with-image")] HttpRequestData req)
//    {
//        _logger.LogInformation("C# HTTP trigger function to add person with image received a request.");

//        var newPerson = new ProductEntity();
//        string? uploadedBlobUrl = null;

//        // 1. Parse the multipart from data
//        var multipartReader = new MultipartReader(req.Headers.GetValues("COntent-Type").First().Split(';')[1].Trim().Split('=')[1], req.Body);
//        var section = await multipartReader.ReadNextSectionAsync();

//        while (section != null)
//        {
//            var contentDisposition = section.Headers["COntent-Disposition"].ToString();
//            var name = contentDisposition.Split(';')[1].Trim().Split('=')[1].Trim('"');

//            if (name == "Name" || name == "Email")
//            {
//                var value = await new StreamReader(section.Body).ReadToEndAsync();
//                if (name == "Name") newPerson.Name = value;
//                if (name == "Email") newPerson.Email = value;
//            }
//            else if (name == "ProfileImage")
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
//        if (string.IsNullOrEmpty(newPerson.Name) || string.IsNullOrEmpty(newPerson.Email) || string.IsNullOrEmpty(uploadedBlobUrl))
//        {
//            return req.CreateResponse(HttpStatusCode.NotFound);
//        }

//        newPerson.PartitionKey = "People";
//        newPerson.RowKey = Guid.NewGuid().ToString();
//        newPerson.ProfilePictureUrl = uploadedBlobUrl;

//        await _tableClient.AddEntityAsync(newPerson);
//        _logger.LogInformation($"Successfully added {newPerson.Name} and uploaded their profile picture.");

//        return req.CreateResponse(HttpStatusCode.Created);
//    }
//}