using ST10438767_CLDV6212.Models;
using ST10438767_CLDV6212.Services;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace ST10438767_CLDV6212
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //--- Add HttpClient toconnect to the api ---
            builder.Services.AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
            });
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); //You can set the timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //register tablestorage with configuration
            builder.Services.AddSingleton(new TableStorageService(configuration.GetConnectionString("AzureStorage")));

            //register tablestorage with configuration
            builder.Services.AddSingleton(new BlobService(configuration.GetConnectionString("AzureStorage")));

            //Regsiter QueueService with configuration
            builder.Services.AddSingleton<QueueService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new QueueService(connectionString, "order");
            });

            //Register fileShareService with configuration
            builder.Services.AddSingleton<AzureFileShareService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new AzureFileShareService(connectionString, "fileshare");
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession(); //It has to go here but no one will car and ai wont help you :)
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

/*
 --REFERENCING LIST--
Debendra Dash, 2024. Creating Shopping Cart Application From Scratch In MVC – Part Two. [Online]. Available at:
https://www.c-sharpcorner.com/article/creating-shopping-cart-application-from-scratch-in-mvc-part2/ [Accessed 13 November 2025]

Gaurav Gupta, 2013. Compare strings using StringComparison.OrdinalIgnoreCase 
[online] Available at: <https://www.c-sharpcorner.com/blogs/compare-strings-using-stringcomparisonordinalignorecase1 > [Accessed 19 August 2025]

IIEVC School of Computer Science, 2025. CLDV6212 Building a Modern Web App with Azure Table Storage & ASP.NET Core MVC - Part 1
[video online] Available at:<https://www.youtube.com/watch?v=Txp7VYUMBGQ&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=3> [Accessed 16 August 2025]. 

IIEVC School of Computer Science, 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 2: Adding Image Uploads with Blob Storage!
[video online] Available at:<https://www.youtube.com/watch?v=CuszKqZvRuM&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=4> [Accessed 16 August 2025]. 

IIEVC School of Computer Science, 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 3: Never Lose Data Again with Queue Storage!
[video online] Available at:<https://www.youtube.com/watch?v=VbZ3Pi63yEc&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=5> [Accessed 17 August 2025]. 

IIEVC School of Computer Science, 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 4: Mastering Azure File Share!
[video online] Available at:<https://www.youtube.com/watch?v=A-mVVL88oEg&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=7> [Accessed 20 August 2025]. 

IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 1 Getting the basics out the way HTTP Trigger
[video online] Available at:<https://www.youtube.com/watch?v=l7s5u-QzYe8&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=7> [Accessed 24 September 2025]. 

IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 2 Azure functions and queues triggers
[video online] Available at:<https://www.youtube.com/watch?v=zP4umzRCsTM&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=8> [Accessed 25 September 2025].

IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 3 Azure functions and MVC
[video online] Available at:<https://www.youtube.com/watch?v=x7yTh85fQbw&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=9> [Accessed 26 September 2025].

IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 4 Azure functions and MVC and blobs
[video online] Available at:<https://www.youtube.com/watch?v=r-VksPFfFpE&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=10> [Accessed 27 September 2025].

IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 5 Azure functions publish
[video online] Available at:<https://www.youtube.com/watch?v=GXGN-aWbwO0&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=11> [Accessed 6 October 2025].

Microsoft Learn, 2024. .NET cryptography model. [Online]. Available at:
https://learn.microsoft.com/en-us/dotnet/standard/security/cryptography-model [Accessed 12 November 2025]

Microsoft Learn, 2022. Hash passwords in ASP.NET Core. [Online]. Available at:
https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-9.0 [Accessed 12 November 2025] 

Microsoft Learn, 2025. Session and state management in ASP.NET Core. [Online]. Available at:
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-9.0 [Accessed 11 November 2025]

Microsoft Learn, 2022. Shopping Cart. [Online]. Available at:
https://learn.microsoft.com/en-us/aspnet/web-forms/overview/getting-started/getting-started-with-aspnet-45-web-forms/shopping-cart [Accessed 13 November 2025]

MicrosoftLearn, 2025. StringComparer.OrdinalIgnoreCase Property 
[online] Available at: <https://learn.microsoft.com/en-us/dotnet/api/system.stringcomparer.ordinalignorecase?view=net-9.0> [Accessed 19 August 2025]

Microsoft, 2025. TableUpdateMode Enum
[online] Available at: <https://learn.microsoft.com/en-us/dotnet/api/azure.data.tables.tableupdatemode?view=azure-dotnet> [Accessed 21 August 2025].

Mustafa Can Sener, 2025. User Registration and Login with .NET Core MVC and Entity Framework. [Online]. Available at:
https://readmedium.com/user-registration-and-login-with-net-core-mvc-and-entity-framework-68793aa97e02 [Accessed 12 November 2025]

Mrzyg?ód, K., 2022. Azure for Developers.

OpenAI. 2025. Chat-GPT (OpenAI's GPT-5-turbo model). [Large language model]. 
Available at: https://chatgpt.com/share/689dd697-22d0-8002-9788-90a36b1cbec0 [Accessed: 14 August 2025]

w3schools, 2025. Create A Filtered Table
[online] Available at: <https://www.w3schools.com/howto/howto_js_filter_table.asp > [Accessed 20 August 2025].

w3schools, 2025. CSS Colors
[online] Available at: <https://www.w3schools.com/css/css_colors.asp> [Accessed 13 August 2025].

w3schools, 2025. CSS Fonts
[online] Available at: <https://www.w3schools.com/css/css_font.asp> [Accessed 13 August 2025].

w3schools, 2025. CSS Gradients
[online] Available at: <https://www.w3schools.com/css/css3_gradients.asp> [Accessed 13 August 2025].

w3schools, 2025. CSS Outline
[online] Available at: <https://www.w3schools.com/css/css_outline.asp> [Accessed 13 August 2025].

w3schools, 2025. CSS Tables
[online] Available at: <https://www.w3schools.com/css/css_table.asp> [Accessed 27 August 2025]

w3schools, 2025. CSS text-align Property
[online] Available at: <https://www.w3schools.com/cssref/pr_text_text-align.php> [Accessed 28 August 2025].

w3schools, 2025. HTML <tbody> Tag
[online] Available at: <https://www.w3schools.com/tags/tag_tbody.asp> [Accessed 27 August 2025].

w3schools, 2025. HTML <thead> Tag
[online] Available at: <https://www.w3schools.com/tags/tag_thead.asp> [Accessed 27 August 2025].

w3schools, 2025. How TO - Center Images
[online] Available at: <https://www.w3schools.com/howto/howto_css_image_center.asp> [Accessed 28 August 2025].
 */
