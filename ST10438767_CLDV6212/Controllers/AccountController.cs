using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using ST10438767_CLDV6212.Models;

namespace ST10438767_CLDV6212.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var jsonContent = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/register", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["RegisterMessage"] = "You’re in. Try not to blow anything up on your first day.";
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Registration failed. Email may already exsist");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var jsonContent = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var user = JsonDocument.Parse(responseBody);
                var email = user.RootElement.GetProperty("Customer_Email").GetString();

                //Set the email in the session
                //Without this line, nothing qill work correctly
                HttpContext.Session.SetString("Customer_Email", email);

                // Login message
                TempData["LoginMessage"] = "You're in. Try not to blow the budget like A-Train on compound V.";

                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, "Login failed. Invalid username or password.");
            return View(model);
        }

        //-- LOGOUT --
        [HttpPost]
        public IActionResult Logout()
        {
            //Clear the session
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
