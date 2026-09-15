using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ST10438767_CLDV6212.Extensions;
using ST10438767_CLDV6212.Models;
using ST10438767_CLDV6212.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ST10438767_CLDV6212.Controllers
{
    public class CustomerViewController : Controller
    {
        
        private readonly ILogger<CustomerViewController> _logger;
        private readonly TableStorageService _tableStorageService;

        public CustomerViewController(ILogger<CustomerViewController> logger, TableStorageService tableStorageService)
        {
            _logger = logger;
            _tableStorageService = tableStorageService;
            //Mrzygłód, K., 2022. Azure for Developers.
        }

        public async Task<IActionResult> Dashboard()
        {
            /*
             Microsoft Learn, 2025. Session and state management in ASP.NET Core. [Online]. Available at:
             https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-9.0 [Accessed 11 November 2025]
             */
            var email = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Index", "Home");

            var products = await _tableStorageService.GetAllProductsAsync();
            return View(products);

            //Mrzygłód, K., 2022. Azure for Developers.
        }

        public async Task<IActionResult> Shop()
        {
            var products = await _tableStorageService.GetAllProductsAsync();
            return View(products);
            //Mrzygłód, K., 2022. Azure for Developers.
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(string productId)
        {
            var products = await _tableStorageService.GetAllProductsAsync();
            var product = products.FirstOrDefault(p => p.Product_ID.ToString() == productId);
            if (product == null) return RedirectToAction("Shop");

            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
            /*
             Microsoft Learn, 2025. Session and state management in ASP.NET Core. [Online]. Available at:
             https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-9.0 [Accessed 11 November 2025]
             */

            var item = cart.FirstOrDefault(c => c.ProductId == product.Product_ID && c.ProductName == product.Product_Name);

            if (item != null)
                item.Quantity++;
            else
                cart.Add(new CartItem
                {
                    CartItemGuid = Guid.NewGuid(),
                    ProductId = product.Product_ID,
                    ProductName = product.Product_Name,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    Quantity = 1
                });
            //Mrzygłód, K., 2022. Azure for Developers.
            /*
             Microsoft Learn, 2025. Session and state management in ASP.NET Core. [Online]. Available at:
             https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-9.0 [Accessed 11 November 2025]
             */
            HttpContext.Session.SetObject("Cart", cart);
            return RedirectToAction("Cart");

            /*
              Microsoft Learn, 2022. Shopping Cart. [Online]. Available at:
              https://learn.microsoft.com/en-us/aspnet/web-forms/overview/getting-started/getting-started-with-aspnet-45-web-forms/shopping-cart [Accessed 13 November 2025]
            */
        }

        public IActionResult Cart()
        {
            /*
             Microsoft Learn, 2025. Session and state management in ASP.NET Core. [Online]. Available at:
             https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-9.0 [Accessed 11 November 2025]
             */
            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
            ViewBag.TotalPrice = cart.Sum(c => c.Price * c.Quantity);
            return View(cart);

            /*
            Microsoft Learn, 2022. Shopping Cart. [Online]. Available at:
            https://learn.microsoft.com/en-us/aspnet/web-forms/overview/getting-started/getting-started-with-aspnet-45-web-forms/shopping-cart [Accessed 13 November 2025]

            Debendra Dash, 2024. Creating Shopping Cart Application From Scratch In MVC – Part Two. [Online]. Available at:
            https://www.c-sharpcorner.com/article/creating-shopping-cart-application-from-scratch-in-mvc-part2/ [Accessed 13 November 2025]
            */
        }

        public async Task<IActionResult> Checkout()
        {
            var email = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Index", "Home");

            /*
             Microsoft Learn, 2025. Session and state management in ASP.NET Core. [Online]. Available at:
             https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-9.0 [Accessed 11 November 2025]
             */

            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
            if (!cart.Any()) return RedirectToAction("Shop");

            //Mrzygłód, K., 2022. Azure for Developers.
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var customer = customers.FirstOrDefault(c => c.Customer_Email == email);

            foreach (var item in cart)
            {
                item.CustomerName = customer?.Customer_Name ?? "";
                item.CustomerEmail = customer?.Customer_Email ?? "";
                item.DeliveryAddress = customer?.Customer_Address ?? "";
            }

            ViewBag.TotalPrice = cart.Sum(c => c.Price * c.Quantity);
            return View(cart);

            /*
            Microsoft Learn, 2022. Shopping Cart. [Online]. Available at:
            https://learn.microsoft.com/en-us/aspnet/web-forms/overview/getting-started/getting-started-with-aspnet-45-web-forms/shopping-cart [Accessed 13 November 2025]

            Debendra Dash, 2024. Creating Shopping Cart Application From Scratch In MVC – Part Two. [Online]. Available at:
            https://www.c-sharpcorner.com/article/creating-shopping-cart-application-from-scratch-in-mvc-part2/ [Accessed 13 November 2025]
            */
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(List<CartItem> cartItems)
        {
            var email = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Index", "Home");

            //Mrzygłód, K., 2022. Azure for Developers.
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var customer = customers.FirstOrDefault(c => c.Customer_Email == email);
            if (customer == null) return RedirectToAction("Index", "Home");

            if (cartItems == null || !cartItems.Any()) return RedirectToAction("Shop");

            foreach (var item in cartItems)
            {
                var order = new Order
                {
                    PartitionKey = "order",
                    RowKey = Guid.NewGuid().ToString(),
                    Customer_Id = customer.Customer_Id,
                    Product_ID = item.ProductId,
                    Quantity = item.Quantity,
                    Order_Status = "Pending",
                    Delivery_Date = DateTime.UtcNow,
                    Delivery_Location = item.DeliveryAddress ?? "Default Location"
                };
                //Mrzygłód, K., 2022. Azure for Developers.

                await _tableStorageService.AddOrderAsync(order);
            }
            /*
             Microsoft Learn, 2025. Session and state management in ASP.NET Core. [Online]. Available at:
             https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-9.0 [Accessed 11 November 2025]
             */

            HttpContext.Session.SetObject("Cart", new List<CartItem>());
            TempData["SuccessMessage"] = "Order placed successfully!";
            return RedirectToAction("Shop");

        }
    }
}

/*
Microsoft Learn, 2022. Shopping Cart. [Online]. Available at:
https://learn.microsoft.com/en-us/aspnet/web-forms/overview/getting-started/getting-started-with-aspnet-45-web-forms/shopping-cart [Accessed 13 November 2025]

Debendra Dash, 2024. Creating Shopping Cart Application From Scratch In MVC – Part Two. [Online]. Available at:
https://www.c-sharpcorner.com/article/creating-shopping-cart-application-from-scratch-in-mvc-part2/ [Accessed 13 November 2025]

Microsoft Learn, 2025. Session and state management in ASP.NET Core. [Online]. Available at:
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-9.0 [Accessed 11 November 2025]
 */


