using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
namespace ST10438767_CLDV6212.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName = "products";

        //set up the services for the blobs
        //this will be to store the images of the products
        public BlobService(string connectionString)
        {
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        //this allows one ot uploade an image byy entering the URL of said image
        public async Task<string> UploadsAsync(Stream fileSteam, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileSteam);
            return blobClient.Uri.ToString();
        }

        //And this of course is to delete the image from the blob container 
        public async Task DeleteBlobAsync(string blobUri)
        {
            Uri uri = new Uri(blobUri);
            //Extracts the blob name from the   URI by taking the last segment of the path
            string blobName = uri.Segments[^1];
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }
    }
}
/*IIEVC School of Computer Science, 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 2: Adding Image Uploads with Blob Storage!
[video online] Available at:<https://www.youtube.com/watch?v=CuszKqZvRuM&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=4> [Accessed 16 August 2025]. */
