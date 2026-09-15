using System.Configuration;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ST10438767_CLDV6212.Models;
using ST10438767_CLDV6212.Services;

namespace ST10438767_CLDV6212.Controllers
{
    public class OrderController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly QueueService _queueService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public OrderController(TableStorageService tableStorageService, QueueService queueService, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _tableStorageService = tableStorageService;
            _queueService = queueService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            var orders = await _tableStorageService.GetAllOrdersAsync();
            return View(orders);
        }

        public async Task<IActionResult> AddOrder()
        {
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var products = await _tableStorageService.GetAllProductsAsync();

            //check for a null or empty list of customers
            if (customers == null || customers.Count == 0)
            {
                //Handle the case where no customers are found
                ModelState.AddModelError("", "No customers found. Please add a customer first.");
            }
            //check for a null or empty list customers
            if (products == null || products.Count == 0)
            {
                //Handle the case where no products are found
                ModelState.AddModelError("", "No customers found. Please add a product first.");
            }

            ViewData["Customer"] = customers;
            ViewData["Product"] = products;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(Order order)
        {
            order.Order_Id = await _tableStorageService.GenerateNextOrderIdAsync(); //This is to generate the next order ID so everything isn't zero
            if (ModelState.IsValid)
            {
                order.Delivery_Date = DateTime.SpecifyKind(order.Delivery_Date, DateTimeKind.Utc);
                order.PartitionKey = "OrderPartition";
                order.RowKey = Guid.NewGuid().ToString();
                await _tableStorageService.AddOrderAsync(order);

                //MessageQueue
                string message = JsonSerializer.Serialize(order);
                await _queueService.SendMessage(message);
                return RedirectToAction("Index");
            }

            //Reload customers and products if the validation fails
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var products = await _tableStorageService.GetAllProductsAsync();

            ViewData["Customer"] = customers;
            ViewData["Product"] = products;
            return View(order);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateOrder(string partitionKey, string rowKey)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var apiBaseUrl = _configuration["FunctionApi:BaseUrl"];
            var httpResponseMessage = await httpClient.GetAsync($"{apiBaseUrl}orders/OrderPartitionKey/{rowKey}");

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var order = await JsonSerializer.DeserializeAsync<Order>(contentStream, options);

            var customers = await _tableStorageService.GetAllCustomersAsync();
            var products = await _tableStorageService.GetAllProductsAsync();
            ViewData["Customer"] = customers;
            ViewData["Product"] = products;
            return View(order);
        }//(microsoft, 2025)(Mrzygłód, 2022)update method that allows the other update method to recieve all information it needs

        [HttpPost]
        public async Task<IActionResult> UpdateOrder(Order order)
        {
            order.Delivery_Date = DateTime.SpecifyKind(order.Delivery_Date, DateTimeKind.Utc);

            var httpClient = _httpClientFactory.CreateClient();
            var apiBaseUrl = _configuration["FunctionApi:BaseUrl"];
            var json = JsonSerializer.Serialize(order);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var httpResponseMessage = await httpClient.PutAsync($"{apiBaseUrl}orders/{order.PartitionKey}/{order.RowKey}", content);

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Failed to update order.");
                var customers = await _tableStorageService.GetAllCustomersAsync();
                var products = await _tableStorageService.GetAllProductsAsync();
                ViewData["Customer"] = customers;
                ViewData["Product"] = products;
                return View(order);
            }

            string message = JsonSerializer.Serialize(order);
            await _queueService.SendMessage(message);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteOrder(string partitionKey, string rowKey, Order order)
        {
            await _tableStorageService.DeleteOrderAsync(partitionKey, rowKey);

            return RedirectToAction("Index");
        }

    }

    /*IIEVC School of Computer Science, 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 3: Never Lose Data Again with Queue Storage!
    [video online] Available at:<https://www.youtube.com/watch?v=VbZ3Pi63yEc&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=5> [Accessed 17 August 2025]. */
}