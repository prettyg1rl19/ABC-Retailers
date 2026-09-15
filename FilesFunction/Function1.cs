using System.Net;
using Azure;
using Azure.Data.Tables;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using HttpMultipartParser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ST10438767_CLDV6212.Models;
using ST10438767_CLDV6212.Services;

namespace FilesFunction;

public class Function1
{
    private readonly ILogger<Function1> _logger;
    private readonly ShareClient _shareClient;
    private readonly string _storageConnectionString;


    public Function1(ILogger<Function1> logger)
    {
        logger = logger;
        _storageConnectionString = Environment.GetEnvironmentVariable("connection");
        _shareClient = new ShareClient(_storageConnectionString, "fileshare");
        _shareClient.CreateIfNotExists();
    }

    [Function("UploadToFileShare")]
    public async Task<HttpResponseData> UploadToFileShare([HttpTrigger(AuthorizationLevel.Function, "post", Route = "uploads")] HttpRequestData req)
    {
        _logger.LogInformation("Processing AddFile request.");

        FileModel fileModel = new FileModel();
        string? fileName = null;
        long fileSize = 0;
        DateTimeOffset? lastModified = null;

        var contentTypeHeader = req.Headers.GetValues("Content-Type").FirstOrDefault();
        var boundary = contentTypeHeader?.Split(';').Select(s => s.Trim())
       .FirstOrDefault(s => s.StartsWith("boundary=", StringComparison.OrdinalIgnoreCase))?.Substring("boundary=".Length);

        if (string.IsNullOrEmpty(boundary))
        {
            _logger.LogWarning("Boundary not found in Content-Type header.");
            return req.CreateResponse(HttpStatusCode.BadRequest);
        }

        var multipartReader = new MultipartReader(boundary, req.Body);
        var section = await multipartReader.ReadNextSectionAsync();

        while (section != null)
        {
            var contentDisposition = section.Headers["Content-Disposition"].ToString();
            var name = contentDisposition?.Split(';')
                .FirstOrDefault(s => s.Trim().StartsWith("name="))?
                .Split('=')[1].Trim('"');

            if (name == "file")
            {
                fileName = contentDisposition.Split(';')
                    .FirstOrDefault(s => s.Trim().StartsWith("filename="))?
                    .Split('=')[1].Trim('"');
                var uniqueFileName = $"{Guid.NewGuid()}-{fileName}";

                var directoryClient = _shareClient.GetDirectoryClient("uploads");
                await directoryClient.CreateIfNotExistsAsync();
                var fileClient = directoryClient.GetFileClient(uniqueFileName);

                using (var ms = new MemoryStream())
                {
                    await section.Body.CopyToAsync(ms);
                    ms.Position = 0;
                    fileSize = ms.Length;
                    lastModified = DateTimeOffset.UtcNow;
                    await fileClient.CreateAsync(ms.Length);
                    await fileClient.UploadRangeAsync(new HttpRange(0, ms.Length), ms);
                }

                fileModel.Name = uniqueFileName;
                fileModel.Size = fileSize;
                fileModel.LastModified = lastModified;
            }
            section = await multipartReader.ReadNextSectionAsync();
        }

        if (string.IsNullOrEmpty(fileModel.Name))
            return req.CreateResponse(HttpStatusCode.BadRequest);

        var response = req.CreateResponse(HttpStatusCode.Created);
        await response.WriteStringAsync($"File uploaded: {fileModel.Name}, Size: {fileModel.DisplaySize}, LastModified: {fileModel.LastModified}");
        return response;
    }

    [Function("ListFiles")]
    public async Task<HttpResponseData> ListFiles([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "File")] HttpRequestData req)
    {
        var directoryClient = _shareClient.GetDirectoryClient("uploads");
        await directoryClient.CreateIfNotExistsAsync();

        var files = new List<FileModel>();
        await foreach (ShareFileItem item in directoryClient.GetFilesAndDirectoriesAsync())
        {
            if (!item.IsDirectory)
            {
                var fileClient = directoryClient.GetFileClient(item.Name);
                var properties = await fileClient.GetPropertiesAsync();
                files.Add(new FileModel
                {
                    Name = item.Name,
                    Size = properties.Value.ContentLength,
                    LastModified = properties.Value.LastModified
                });
            }
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(files);
        return response;
    }


    [Function("DownloadFromFileShare")]
    public async Task<HttpResponseData> DownloadFromFileShare([HttpTrigger(AuthorizationLevel.Function, "get", Route = "download/{fileName}")] HttpRequestData req, string fileName)
    {
        try
        {
            _logger.LogInformation($"Attempting to download file: {fileName}");
            var directoryClient = _shareClient.GetDirectoryClient("uploads");
            var fileClient = directoryClient.GetFileClient(fileName);

            var exists = await fileClient.ExistsAsync();
            _logger.LogInformation($"File exists: {exists}");

            if (!exists)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync($"File '{fileName}' not found");
                return notFoundResponse;
            }

            var downloadInfo = await fileClient.DownloadAsync();
            var response = req.CreateResponse(HttpStatusCode.OK);

            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                await downloadInfo.Value.Content.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
            response.Headers.Add("Content-Type", "application/octet-stream");
            response.Headers.Add("Content-Length", fileBytes.Length.ToString());

            await response.WriteBytesAsync(fileBytes);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error downloading file '{fileName}': {ex.Message}");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync($"Internal Server Error: {ex.Message}");
            return errorResponse;
        }
    }
}