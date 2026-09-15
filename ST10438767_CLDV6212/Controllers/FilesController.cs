using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ST10438767_CLDV6212.Models;
using ST10438767_CLDV6212.Services;

namespace ST10438767_CLDV6212.Controllers
{
    public class FilesController : Controller
    {
        private readonly AzureFileShareService _fileShareService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public FilesController(AzureFileShareService fileShareService, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _fileShareService = fileShareService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["FunctionApi:BaseUrl"];
            try
            { 
                var respFile = await client.GetAsync($"{baseUrl}ListFiles");


                if (respFile.IsSuccessStatusCode)
                {
                    using var contentStream = await respFile.Content.ReadAsStreamAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var allfiles = await JsonSerializer.DeserializeAsync<IEnumerable<FileModel>>(contentStream, options);
                    return View(allfiles);
                }

                }
                catch (HttpRequestException)
                {
                    ViewBag.ErrorMessage = "Could not connect to the API. Please ensure the Azure Function is running.";
                    return View(new List<FileModel>());
                }

                ViewBag.ErrorMessage = "An error occured while retrieving data from the API.";
            return View(new List<FileModel>());
        }

        [HttpGet]
        public IActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file to upload");
                return RedirectToAction("Index");
            }

            try
            {
                using (var stream = file.OpenReadStream())//(IIEVC School of Computer Science, 2025)
                {
                    string directoryName = "uploads";
                    string fileName = file.FileName;
                    await _fileShareService.UploadFileAsync(directoryName, fileName, stream);
                }
                TempData["Message"] = $"File '{file.FileName}' uploaded successfully";
            }
            catch (Exception e)
            {
                TempData["Message"] = $"File upload failed: {e.Message}";
            }
            return RedirectToAction("Index");
        }
        

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return BadRequest("File name cannot be null or empty");
            }
            try
            {
                var fileStream = await _fileShareService.DownLoadFileAsync("uploads", fileName);
                if (fileStream == null)
                {
                    return NotFound($"File '{fileName}' not found");
                }
                return File(fileStream, "application/octet-stream", fileName);
            }
            catch (Exception e)
            {
                return BadRequest($"Error downloading file: {e.Message}");
            }
        }
    }

    /*IIEVC School of Computer Science, 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 4: Mastering Azure File Share!
[video online] Available at:<https://www.youtube.com/watch?v=A-mVVL88oEg&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=7> [Accessed 20 August 2025]. */
}
