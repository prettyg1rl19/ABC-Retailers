using System.Text.Json;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using ST10438767_CLDV6212.Models;
using ST10438767_CLDV6212.Services;

namespace ST10438767_CLDV6212.Controllers
{
    public class ProductController : Controller
    {
        private readonly BlobService _blobService;
        private readonly TableStorageService _tableStorageService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ProductController(BlobService blobService, TableStorageService tableStorageService,
            IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _blobService = blobService;
            _tableStorageService = tableStorageService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _tableStorageService.GetAllProductsAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product products, IFormFile file)
        {
            var apiBaseUrl = _configuration["FunctionApi:BaseUrl"];
            if (string.IsNullOrEmpty(apiBaseUrl))
                throw new InvalidOperationException("API Base URL is not configured.");

            try
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(products.Product_Name ?? ""), "productName");
                formData.Add(new StringContent(products.Description ?? ""), "productDescription");
                formData.Add(new StringContent(products.Price.ToString()), "productPrice");

                if (file != null)
                    formData.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.PostAsync($"{apiBaseUrl}products", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = $"Product {products.Product_Name} added successfully.";
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Error adding product with image.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }

            return View(products);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteProduct(string partitionKey, string rowKey, Product products)
        {
            if (products != null && !string.IsNullOrEmpty(products.ImageUrl))
            {
                await _blobService.DeleteBlobAsync(products.ImageUrl);
            }

            await _tableStorageService.DeleteProductAsync(partitionKey, rowKey);

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> EditProduct(string partitionKey, string rowKey, Product product)
        {
            product.Product_ID = await _tableStorageService.GenerateNextProductIdAsync();
            await _tableStorageService.EditProductAsync(partitionKey, rowKey, product);
            return RedirectToAction("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(int productId)
        {
            var products = await _tableStorageService.GetAllProductsAsync();
            var prod = products.ElementAt(productId);
            return View(prod);
        }

    }
}
/*
MicrosoftLearn, 2025. StringComparer.OrdinalIgnoreCase Property [online] Available at: 
< https://learn.microsoft.com/en-us/dotnet/api/system.stringcomparer.ordinalignorecase?view=net-9.0 > [Accessed 19 August 2025]

Gaurav Gupta, 2013. Compare strings using StringComparison.OrdinalIgnoreCase [online] Available at:
< https://www.c-sharpcorner.com/blogs/compare-strings-using-stringcomparisonordinalignorecase1 > [Accessed 19 August 2025]
*/