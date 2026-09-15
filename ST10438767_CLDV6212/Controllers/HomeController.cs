using Microsoft.AspNetCore.Mvc;
using ST10438767_CLDV6212.Models;
using ST10438767_CLDV6212.Services;
using System.Diagnostics;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ST10438767_CLDV6212.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly TableStorageService _tableService;
        private const string AdminEmailSuffix = "@admin.com";

        public HomeController(ApplicationDbContext dbContext, TableStorageService tableService)
        {
            _dbContext = dbContext;
            _tableService = tableService;
            //Mrzyg?ód, K., 2022. Azure for Developers.
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
            //Mrzyg?ód, K., 2022. Azure for Developers.
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            /*
             Mustafa Can Sener, 2025. User Registration and Login with .NET Core MVC and Entity Framework. [Online]. Available at:
             https://readmedium.com/user-registration-and-login-with-net-core-mvc-and-entity-framework-68793aa97e02 [Accessed 12 November 2025]

             Dot Net Tutorials, 2025. Register, Login, and Logout using ASP.NET Core Identity. [Online]. Available at:
             https://dotnettutorials.net/lesson/register-login-logout-in-asp-net-core-identity/ [Accessed 12 November 2025]
             */

            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Email is required.";
                return View("Index");
            }

            email = email.Trim();
            var isAdmin = email.EndsWith(AdminEmailSuffix, System.StringComparison.OrdinalIgnoreCase);

            if (isAdmin)
            {
                var adminSql = await _dbContext.Customers.FirstOrDefaultAsync(c => c.customerEmail.ToLower() == email.ToLower());

                if (adminSql == null)
                {
                    adminSql = new Customers
                    {
                        customerEmail = email,
                        customerName = "Admin",
                        Role = "Admin"
                    };
                    //Mrzyg?ód, K., 2022. Azure for Developers.

                    _dbContext.Customers.Add(adminSql);
                    await _dbContext.SaveChangesAsync();
                    //Mrzyg?ód, K., 2022. Azure for Developers.

                    TempData["EmailToSet"] = email;
                    return RedirectToAction(nameof(PasswordSet));
                }

                if (string.IsNullOrEmpty(adminSql.PasswordHash))
                {
                    TempData["EmailToSet"] = email;
                    return RedirectToAction(nameof(PasswordSet));
                }

                if (!VerifyPassword(password, adminSql.PasswordHash))
                {
                    ViewBag.Error = "Invalid login credentials.";
                    return View("Index");
                    //Mrzyg?ód, K., 2022. Azure for Developers.
                }

                HttpContext.Session.SetString("Username", adminSql.customerEmail);
                HttpContext.Session.SetString("Role", "Admin");
                HttpContext.Session.SetString("UserId", adminSql.customerId.ToString());

                return RedirectToAction("Index", "Customer");
                /*
                 Microsoft Learn, 2025. Session and state management in ASP.NET Core. [Online]. Available at:
                 https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-9.0 [Accessed 11 November 2025]
                 */
            }

            var azureCustomer = await _tableService.GetAllCustomersAsync();
            //Mrzyg?ód, K., 2022. Azure for Developers.

            var customerExists = azureCustomer.Any(c => c.Customer_Email.ToLower() == email.ToLower());

            if (!customerExists)
            {
                TempData["EmailToRegister"] = email;
                return RedirectToAction(nameof(Register));
            }

            var sqlCustomer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.customerEmail.ToLower() == email.ToLower());
            if (sqlCustomer == null)
            {
                var customer = azureCustomer.First(c => c.Customer_Email.ToLower() == email.ToLower());
                sqlCustomer = new Customers
                {
                    customerEmail = customer.Customer_Email,
                    customerName = customer.Customer_Name,
                    customerAddress = customer.Customer_Address,
                    Role = "Customer"
                };
                _dbContext.Customers.Add(sqlCustomer);
                await _dbContext.SaveChangesAsync();
                //Mrzyg?ód, K., 2022. Azure for Developers.
            }

            if (string.IsNullOrEmpty(sqlCustomer.PasswordHash))
            {
                TempData["EmailToSet"] = email;
                return RedirectToAction(nameof(PasswordSet));
            }

            if (!VerifyPassword(password, sqlCustomer.PasswordHash))
            {
                ViewBag.Error = "Invalid login credentials.";
                return View("Index");
            }

            HttpContext.Session.SetString("Username", sqlCustomer.customerEmail);
            HttpContext.Session.SetString("Role", "Customer");
            HttpContext.Session.SetString("UserId", sqlCustomer.customerId.ToString());
            /*
             Microsoft Learn, 2025. Session and state management in ASP.NET Core. [Online]. Available at:
             https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-9.0 [Accessed 11 November 2025]

             Mustafa Can Sener, 2025. User Registration and Login with .NET Core MVC and Entity Framework. [Online]. Available at:
             https://readmedium.com/user-registration-and-login-with-net-core-mvc-and-entity-framework-68793aa97e02 [Accessed 12 November 2025]

             Dot Net Tutorials, 2025. Register, Login, and Logout using ASP.NET Core Identity. [Online]. Available at:
             https://dotnettutorials.net/lesson/register-login-logout-in-asp-net-core-identity/ [Accessed 12 November 2025]
         
            */

            return RedirectToAction("CustomerIndex", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string customerName, string customerEmail,  string customerAddress)
        {
            var allAzureCustomers = await _tableService.GetAllCustomersAsync();
            var existingAzureCustomer = allAzureCustomers.FirstOrDefault(c => c.Customer_Email?.ToLower() == customerEmail.ToLower());

            if (existingAzureCustomer != null)
            {
                ViewBag.Error = "Customer already exists. Please login.";
                return View();
            }

            var sqlCustomer = new Customers
            {
                customerEmail = customerEmail,
                customerName = customerName,
                customerAddress = customerAddress,
                Role = "Customer",
                PasswordHash = null
            };
            _dbContext.Customers.Add(sqlCustomer);
            await _dbContext.SaveChangesAsync();
            //Mrzyg?ód, K., 2022. Azure for Developers.

            var azureCustomer = new Customer
            {
                PartitionKey = "Customer",
                RowKey = Guid.NewGuid().ToString(),
                Customer_Id = sqlCustomer.customerId,
                Customer_Email = customerEmail,
                Customer_Name = customerName,
                Customer_Address = customerAddress
            };
            await _tableService.AddCustomerAsync(azureCustomer);
            //Mrzyg?ód, K., 2022. Azure for Developers.

            TempData["EmailToSetPassword"] = customerEmail;
            return RedirectToAction(nameof(PasswordSet));
        }

        /*
         Mustafa Can Sener, 2025. User Registration and Login with .NET Core MVC and Entity Framework. [Online]. Available at:
         https://readmedium.com/user-registration-and-login-with-net-core-mvc-and-entity-framework-68793aa97e02 [Accessed 12 November 2025]

         Dot Net Tutorials, 2025. Register, Login, and Logout using ASP.NET Core Identity. [Online]. Available at:
         https://dotnettutorials.net/lesson/register-login-logout-in-asp-net-core-identity/ [Accessed 12 November 2025]
         */

        [HttpGet]
        public IActionResult PasswordSet()
        {
            ViewBag.Email = TempData["EmailToSet"] as string ?? TempData["EmailToSetPassword"] as string ?? "";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordSet(string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                ViewBag.Email = email;
                return View();
            }

            var sqlCustomer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.customerEmail.ToLower() == email.ToLower());
            if (sqlCustomer == null)
            {
                sqlCustomer = new Customers
                {
                    customerEmail = email,
                    Role = email.EndsWith(AdminEmailSuffix) ? "Admin" : "Customer",
                    customerAddress = "N/A"
                };
                _dbContext.Customers.Add(sqlCustomer);
                //Mrzyg?ód, K., 2022. Azure for Developers.
            }

            sqlCustomer.PasswordHash = HashPassword(password);
            await _dbContext.SaveChangesAsync();
            //Mrzyg?ód, K., 2022. Azure for Developers.

            return RedirectToAction("Index");

            /*
             Microsoft Learn, 2022. Hash passwords in ASP.NET Core. [Online]. Available at:
             https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-9.0 [Accessed 12 November 2025] 
             */
        }

        public async Task<IActionResult> AdminIndex()
        {
            var customers = await _tableService.GetAllCustomersAsync();
            return View(customers);
            //Mrzyg?ód, K., 2022. Azure for Developers.
        }
        public async Task<IActionResult> CustomerIndex()
        {
            var customers = await _tableService.GetAllCustomersAsync();
            return View(customers);
            //Mrzyg?ód, K., 2022. Azure for Developers.
        }

        public IActionResult CustomerView()
        {
            return View();
            //Mrzyg?ód, K., 2022. Azure for Developers.
        }

        /*
         Microsoft Learn, 2022. Hash passwords in ASP.NET Core. [Online]. Available at:
         https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-9.0 [Accessed 12 November 2025] 
         */
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(sha.ComputeHash(bytes));
            /*
             Microsoft Learn, 2024. .NET cryptography model. [Online]. Available at:
             https://learn.microsoft.com/en-us/dotnet/standard/security/cryptography-model [Accessed 12 November 2025]
             */
        }

        private bool VerifyPassword(string input, string hash) => HashPassword(input) == hash;
    }
}

/*
Dot Net Tutorials, 2025. Register, Login, and Logout using ASP.NET Core Identity. [Online]. Available at:
https://dotnettutorials.net/lesson/register-login-logout-in-asp-net-core-identity/ [Accessed 12 November 2025]

Microsoft Learn, 2024. .NET cryptography model. [Online]. Available at:
https://learn.microsoft.com/en-us/dotnet/standard/security/cryptography-model [Accessed 12 November 2025]

Microsoft Learn, 2022. Hash passwords in ASP.NET Core. [Online]. Available at:
https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-9.0 [Accessed 12 November 2025] 

Microsoft Learn, 2025. Session and state management in ASP.NET Core. [Online]. Available at:
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-9.0 [Accessed 11 November 2025]

Mustafa Can Sener, 2025. User Registration and Login with .NET Core MVC and Entity Framework. [Online]. Available at:
https://readmedium.com/user-registration-and-login-with-net-core-mvc-and-entity-framework-68793aa97e02 [Accessed 12 November 2025]

Mrzyg?ód, K., 2022. Azure for Developers.
*/

