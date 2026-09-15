using System;
using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using ST10438767_CLDV6212.Models;
using ST10438767_CLDV6212.Services;

namespace ST10438767_CLDV6212.Controllers
{
    public class CustomerController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CustomerController(TableStorageService tableStorageService, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _tableStorageService = tableStorageService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;

            //Mrzygłód, K., 2022. Azure for Developers.
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _tableStorageService.GetAllCustomersAsync();
            return View(customers);
            //Mrzygłód, K., 2022. Azure for Developers.
        }


        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            try
            {
                customer.Customer_Id = await _tableStorageService.GenerateNextCustomerIdAsync();
                customer.PartitionKey = "CustomerPartition";
                customer.RowKey = Guid.NewGuid().ToString();

                await _tableStorageService.AddCustomerAsync(customer);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Failed to add customer: {ex.Message}";
                return View("AddCustomerAsync", customer);
            }

            return RedirectToAction("Index");
            //Mrzygłód, K., 2022. Azure for Developers.
        }

        [HttpGet]
        public IActionResult AddCustomerAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(string partitionKey, string rowKey, Customer customer)
        {
            await _tableStorageService.DeleteCustomerAsync(partitionKey, rowKey);

            return RedirectToAction("Index");
            //Mrzygłód, K., 2022. Azure for Developers.
        }

        public async Task<IActionResult> EditCustomer(string partitionKey, string rowKey, Customer customer)
        {
            //customer.customerId = await _tableStorageService.GenerateCustomerIdAsync();
            await _tableStorageService.EditCustomerAsync(partitionKey, rowKey, customer);
            return RedirectToAction("Edit");

            //Mrzygłód, K., 2022. Azure for Developers.
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(int customerId)
        {
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var cust = customers.ElementAt(customerId);
            return View(cust);
            //Mrzygłód, K., 2022. Azure for Developers.
        }

        [HttpPost]
        public async Task<IActionResult> Search(string customerName, string customerEmail, string customerAddress)
        {
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var query = customers.AsQueryable();
            /*
             MicrosoftLearn, 2025. StringComparer.OrdinalIgnoreCase Property
             Gaurav Gupta, 2013. Compare strings using StringComparison.OrdinalIgnoreCase
             */
            // Performs a case-insensitive comparison using the binary Unicode values of characters.
            // This ensures that searches match regardless of letter casing (e.g., "apple" matches "Apple").
            if (!string.IsNullOrEmpty(customerName))
                query = query.Where(c => c.Customer_Name.Contains(customerName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(customerEmail))
                query = query.Where(c => c.Customer_Email.Contains(customerEmail, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(customerAddress))
                query = query.Where(c => c.Customer_Address.Contains(customerAddress, StringComparison.OrdinalIgnoreCase));

            return View("Index", query.ToList());
        }
    }
}
/*
Gaurav Gupta, 2013. Compare strings using StringComparison.OrdinalIgnoreCase [online] Available at:
< https://www.c-sharpcorner.com/blogs/compare-strings-using-stringcomparisonordinalignorecase1 > [Accessed 19 August 2025]

MicrosoftLearn, 2025. StringComparer.OrdinalIgnoreCase Property [online] Available at: 
< https://learn.microsoft.com/en-us/dotnet/api/system.stringcomparer.ordinalignorecase?view=net-9.0 > [Accessed 19 August 2025]

Mrzygłód, K., 2022. Azure for Developers.
*/

