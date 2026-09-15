<img width="1473" height="863" alt="Screenshot 2025-11-14 170025" src="https://github.com/user-attachments/assets/eeb44504-fa6b-4ceb-9ec9-0fdbcedcf86a" /># 🌸 ABC Retailers

## Cloud-Based Retail Web Application

**ABC Retailers** is a cloud-enabled online retail platform developed using **ASP.NET Core MVC and Microsoft Azure**.

The application provides separate experiences for **customers and administrators**, allowing customers to browse products, manage a shopping cart and track orders, while administrators can manage customers, orders and uploaded documents.

The project demonstrates the integration of a web application with multiple Azure cloud services, including **Azure Functions, Blob Storage, Queue Storage and Table Storage**.

---

## 🌐 Live Website

**https://st10438767cldv6212wa-cqgwdtawhqfxhda0.canadacentral-01.azurewebsites.net**

> **Note:** This website is no longer available due to the hosting having stopped.

---

## 🎥 Demonstration Video

**[Watch the ABC Retailers Feature Demonstration](https://youtu.be/FKogGm8YjHI)**

> **Note:** This video is an additional demonstration showcasing one feature of the application. The main project demonstration is included in the submitted Word document.

---

# ✨ Features

ABC Retailers provides different functionality depending on the user's role.

## 🛍️ Customer Experience

Customers can:

* Sign in to the application
* Enter the online shop
* Browse available products
* Add products to their shopping cart
* View their cart
* View their orders
* Check their order status
* Log out securely

Customers only have access to their own customer functionality and cannot access administrative operations.

## 🥀 User Interface

<img width="708" height="554" alt="Screenshot 2025-11-14 165735" src="https://github.com/user-attachments/assets/548e518a-3653-43aa-810c-8665a4602e94" />

<img width="668" height="557" alt="Screenshot 2025-11-14 165823" src="https://github.com/user-attachments/assets/4beb62a6-0f44-48db-9703-673187add141" />

<img width="673" height="557" alt="Screenshot 2025-11-14 165929" src="https://github.com/user-attachments/assets/5e37c038-3eed-4006-8300-8c4b64f4982b" />

<img width="808" height="380" alt="Screenshot 2025-11-14 170013" src="https://github.com/user-attachments/assets/1413356c-5c2e-4df4-93b6-a305c2b3effc" />

<img width="1473" height="863" alt="Screenshot 2025-11-14 170025" src="https://github.com/user-attachments/assets/e0dc0666-cf56-4c03-a469-4bd621e47e09" />

<img width="1346" height="624" alt="Screenshot 2025-11-14 170416" src="https://github.com/user-attachments/assets/90918e96-22b2-4c58-9994-d56b4c3236c9" />


---

## 👩‍💼 Administrator Experience

Administrators have access to additional management functionality.

Administrators can:

* Sign in as an administrator
* View customer orders
* Add customers
* Delete customers
* View customers
* Edit orders
* Delete orders
* View uploaded documents
* Upload files associated with orders
* Log out

Administrative functionality is separated from the customer shopping experience.

## 🪿 User Interfaces

<img width="708" height="554" alt="Screenshot 2025-11-14 165735" src="https://github.com/user-attachments/assets/1bf27233-2212-4581-bec9-9de66da79d37" />

<img width="668" height="557" alt="Screenshot 2025-11-14 165823" src="https://github.com/user-attachments/assets/654b123d-d704-48f3-b0c3-30dc7c417cea" />

<img width="673" height="557" alt="Screenshot 2025-11-14 165929" src="https://github.com/user-attachments/assets/bc070d8b-a1fe-4860-af89-6d951ed404de" />

<img width="1660" height="515" alt="Screenshot 2025-11-14 170542" src="https://github.com/user-attachments/assets/d4d29b87-6ddd-4810-89df-25f518846bc5" />

<img width="1769" height="674" alt="Screenshot 2025-11-14 170652" src="https://github.com/user-attachments/assets/ef7aa632-7245-45f6-9ce2-f6c9db848dab" />

<img width="1674" height="566" alt="Screenshot 2025-11-14 170718" src="https://github.com/user-attachments/assets/94b07fa8-42df-4a6b-a890-ffcd891d74f5" />

<img width="1672" height="661" alt="Screenshot 2025-11-14 170819" src="https://github.com/user-attachments/assets/2855d339-9d55-4445-a987-d090b8c36992" />

---

# ☁️ Cloud Architecture

The application makes use of several Microsoft Azure services to provide cloud-based functionality.

```text
                         ┌─────────────────────┐
                         │       Customer      │
                         │       Browser       │
                         └──────────┬──────────┘
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │   ASP.NET Core MVC  │
                         │   Web Application   │
                         └──────────┬──────────┘
                                    │
             ┌──────────────────────┼──────────────────────┐
             │                      │                      │
             ▼                      ▼                      ▼
     ┌───────────────┐      ┌───────────────┐      ┌───────────────┐
     │ Azure         │      │ Azure Storage │      │ Azure Queue   │
     │ Functions     │      │ Services      │      │ Storage       │
     └───────┬───────┘      └───────┬───────┘      └───────────────┘
             │                      │
       ┌─────┼─────┐          ┌─────┼─────┐
       │     │     │          │     │     │
       ▼     ▼     ▼          ▼     ▼     ▼
    Login  Orders Products  Blob  Tables Files
```

---

# ⚡ Azure Functions

The solution contains multiple Azure Function projects responsible for specific cloud-based operations.

These include functionality relating to:

* Authentication
* Products
* Orders
* Files
* Blob storage
* Queue processing
* Table storage

This approach separates individual cloud operations from the main MVC application.

### Azure Function Projects

The repository contains projects including:

* `BlobFunction`
* `FilesFunction`
* `LoginAPI`
* `OrderFunction`
* `POEFunctions`
* `ProductFunction`
* `QueueFunction`
* `TableFunction`
* `Table_Function`
* `TestQueue`

---

# 🗄️ Azure Storage

Different Azure storage technologies are used for different requirements within the application.

### Azure Blob Storage

Used for handling uploaded files and documents associated with the application.

### Azure Table Storage

Used for storing structured application data using Azure Table Storage.

### Azure Queue Storage

Used to support asynchronous processing and communication between components.

### Azure File Storage

Used as part of the application's cloud file-management functionality.

---

# 🏗️ Application Architecture

The project follows an **ASP.NET Core MVC architecture** for the main web application.

```text
ABC-Retailers
│
├── ABC_Retailers
│   ├── Controllers
│   ├── Models
│   ├── Views
│   ├── Services
│   ├── wwwroot
│   └── Configuration
│
├── BlobFunction
│
├── FilesFunction
│
├── LoginAPI
│
├── OrderFunction
│
├── POEFunctions
│
├── ProductFunction
│
├── QueueFunction
│
├── TableFunction
│
├── Table_Function
│
└── TestQueue
```

The MVC application is responsible for the user-facing experience, while Azure Functions provide supporting cloud functionality.

---

# 🧰 Technologies Used

| Technology              | Purpose                              |
| ----------------------- | ------------------------------------ |
| **C#**                  | Application development              |
| **ASP.NET Core MVC**    | Web application framework            |
| **Razor**               | Dynamic web views                    |
| **HTML5**               | Application structure                |
| **CSS3**                | User interface and styling           |
| **JavaScript**          | Client-side functionality            |
| **Microsoft Azure**     | Cloud platform                       |
| **Azure Functions**     | Serverless application functionality |
| **Azure Blob Storage**  | File and document storage            |
| **Azure Table Storage** | Cloud data storage                   |
| **Azure Queue Storage** | Asynchronous processing              |
| **Azure File Storage**  | Cloud file management                |
| **Git/GitHub**          | Version control                      |

---

# 🔐 Role-Based Functionality

ABC Retailers provides different privileges according to the authenticated user's role.

| Functionality         | Customer | Administrator |
| --------------------- | :------: | :-----------: |
| Sign in               |     ✅    |       ✅       |
| Browse shop           |     ✅    |       ❌       |
| Add products to cart  |     ✅    |       ❌       |
| View cart             |     ✅    |       ❌       |
| View own order status |     ✅    |       ❌       |
| View customer orders  |     ❌    |       ✅       |
| Add customers         |     ❌    |       ✅       |
| Delete customers      |     ❌    |       ✅       |
| Edit orders           |     ❌    |       ✅       |
| Delete orders         |     ❌    |       ✅       |
| View documents        |     ❌    |       ✅       |
| Upload order files    |     ❌    |       ✅       |

This separation ensures that customers cannot access functionality intended for administrators.

---

# 🛒 Shopping Workflow

The primary customer workflow follows a simple retail process:

```text
Sign In
   │
   ▼
Browse Shop
   │
   ▼
Select Products
   │
   ▼
Add to Cart
   │
   ▼
View Cart
   │
   ▼
Place Order
   │
   ▼
Track Order Status
```

---

# 📦 Order Management

Orders can be managed through both the customer and administrative sides of the application.

### Customers

Customers can:

* View their orders
* Review order information
* Monitor order status

### Administrators

Administrators can:

* View orders
* Edit orders
* Delete orders
* Upload files associated with orders
* View associated documents

---

# 📁 File & Document Management

ABC Retailers incorporates cloud-based file functionality for documents associated with orders.

Uploaded files are handled through Azure-backed functionality, allowing files to be stored independently from the main web application.

This demonstrates the use of cloud storage for managing unstructured application data.

---

# 🎨 User Interface

The interface is designed around a retail-focused experience with separate customer and administrator workflows.

The customer interface prioritises:

* Product discovery
* Clear navigation
* Shopping-cart interaction
* Order visibility
* Simple user flows

The administrative interface prioritises:

* Data management
* Order management
* Customer management
* Document management
* Administrative controls

---

# 🚀 Getting Started

## Prerequisites

To run the project locally, you should have:

* Visual Studio 2022 or later
* A compatible .NET SDK
* An Azure account where required
* Git
* Access to the required Azure Storage services

---

## Clone the Repository

```bash
git clone https://github.com/prettyg1rl19/ABC-Retailers.git
```

```bash
cd ABC-Retailers
```

---

## Open the Solution

Open:

```text
ST10438767_CLDV6212.sln
```

in Visual Studio.

Restore the required NuGet packages and ensure that the appropriate Azure configuration is available for the cloud services used by the application.

---

## Configuration

Azure connection strings, storage settings and other environment-specific configuration should be stored securely.

Do **not** commit:

* Passwords
* API keys
* Connection strings containing credentials
* Azure secrets
* Other sensitive configuration

For local development, use the appropriate ASP.NET Core configuration mechanisms or environment variables.

---

# 🧪 Testing the Application

## Customer Workflow

1. Open the application.
2. Sign in as a customer.
3. Enter the shop.
4. Browse available products.
5. Add products to the cart.
6. View the cart.
7. Place or view an order.
8. Check the order status.
9. Log out.

## Administrator Workflow

1. Open the application.
2. Sign in as an administrator.
3. Access the administrative functionality.
4. View customers and orders.
5. Add or delete customers.
6. Edit or delete orders.
7. View documents.
8. Upload order-related files.
9. Log out.

---

# 📚 References

The following resources were consulted during the development of ABC Retailers.

### C# & ASP.NET MVC

**Dash, D. (2024).** Creating Shopping Cart Application From Scratch In MVC – Part Two. *C# Corner*.
https://www.c-sharpcorner.com/article/creating-shopping-cart-application-from-scratch-in-mvc-part2/

**Gupta, G. (2013).** Compare strings using StringComparison.OrdinalIgnoreCase. *C# Corner*.
https://www.c-sharpcorner.com/blogs/compare-strings-using-stringcomparisonordinalignorecase1

**Sener, M. C. (2025).** User Registration and Login with .NET Core MVC and Entity Framework. *Medium*.
https://readmedium.com/user-registration-and-login-with-net-core-mvc-and-entity-framework-68793aa97e02

---

### Microsoft Learn

**Microsoft Learn. (2024).** .NET cryptography model.
https://learn.microsoft.com/en-us/dotnet/standard/security/cryptography-model

**Microsoft Learn. (2022).** Hash passwords in ASP.NET Core.
https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing

**Microsoft Learn. (2025).** Session and state management in ASP.NET Core.
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state

**Microsoft Learn. (2022).** Shopping Cart.
https://learn.microsoft.com/en-us/aspnet/web-forms/overview/getting-started/getting-started-with-aspnet-45-web-forms/shopping-cart

**Microsoft Learn. (2025).** StringComparer.OrdinalIgnoreCase Property.
https://learn.microsoft.com/en-us/dotnet/api/system.stringcomparer.ordinalignorecase

**Microsoft. (2025).** TableUpdateMode Enum.
https://learn.microsoft.com/en-us/dotnet/api/azure.data.tables.tableupdatemode

---

### Azure & Cloud Development Tutorials

**IIEVC School of Computer Science. (2025).** CLDV6212 Building a Modern Web App with Azure Table Storage & ASP.NET Core MVC – Part 1.
https://www.youtube.com/watch?v=Txp7VYUMBGQ

**IIEVC School of Computer Science. (2025).** CLDV6212 ASP.NET MVC & Azure Series – Part 2: Adding Image Uploads with Blob Storage!
https://www.youtube.com/watch?v=CuszKqZvRuM

**IIEVC School of Computer Science. (2025).** CLDV6212 ASP.NET MVC & Azure Series – Part 3: Never Lose Data Again with Queue Storage!
https://www.youtube.com/watch?v=VbZ3Pi63yEc

**IIEVC School of Computer Science. (2025).** CLDV6212 ASP.NET MVC & Azure Series – Part 4: Mastering Azure File Share!
https://www.youtube.com/watch?v=A-mVVL88oEg

**IIE Emeris School of Computer Science. (2025).** CLDV6212 Azure Functions Part 1: Getting the Basics Out the Way — HTTP Trigger.
https://www.youtube.com/watch?v=l7s5u-QzYe8

**IIE Emeris School of Computer Science. (2025).** CLDV6212 Azure Functions Part 2: Azure Functions and Queues Triggers.
https://www.youtube.com/watch?v=zP4umzRCsTM

**IIE Emeris School of Computer Science. (2025).** CLDV6212 Azure Functions Part 3: Azure Functions and MVC.
https://www.youtube.com/watch?v=x7yTh85fQbw

**IIE Emeris School of Computer Science. (2025).** CLDV6212 Azure Functions Part 4: Azure Functions, MVC and Blobs.
https://www.youtube.com/watch?v=r-VksPFfFpE

**IIE Emeris School of Computer Science. (2025).** CLDV6212 Azure Functions Part 5: Azure Functions Publish.
https://www.youtube.com/watch?v=GXGN-aWbwO0

---

### Azure for Developers

**Mrzygłód, K. (2022).** *Azure for Developers.*

---

### Web Development

**W3Schools. (2025).** Create a Filtered Table.
https://www.w3schools.com/howto/howto_js_filter_table.asp

**W3Schools. (2025).** CSS Colors.
https://www.w3schools.com/css/css_colors.asp

**W3Schools. (2025).** CSS Fonts.
https://www.w3schools.com/css/css_font.asp

**W3Schools. (2025).** CSS Gradients.
https://www.w3schools.com/css/css3_gradients.asp

**W3Schools. (2025).** CSS Outline.
https://www.w3schools.com/css/css3_outline.asp

**W3Schools. (2025).** CSS Tables.
https://www.w3schools.com/css/css_table.asp

**W3Schools. (2025).** CSS text-align Property.
https://www.w3schools.com/cssref/pr_text_text-align.php

**W3Schools. (2025).** HTML `<tbody>` Tag.
https://www.w3schools.com/tags/tag_tbody.asp

**W3Schools. (2025).** HTML `<thead>` Tag.
https://www.w3schools.com/tags/tag_thead.asp

**W3Schools. (2025).** How To — Center Images.
https://www.w3schools.com/howto/howto_css_image_center.asp

---

# 🤖 AI Usage Disclosure

AI tools were used during development as an **assistive resource**, rather than as a replacement for the development process.

ChatGPT was used for:

1. Finding potential slogan ideas for the website.
2. Exploring colour-scheme ideas. The suggested colour schemes were ultimately discarded.
3. Troubleshooting the customer deletion operation and checking whether the implementation approach was correct.

The final implementation was developed, tested and corrected by the project author.

### AI Disclosure

**OpenAI. (2025).** ChatGPT (GPT-5-turbo). [Large language model].
https://chatgpt.com/share/689dd697-22d0-8002-9788-90a36b1cbec0

---

# 👩‍💻 Project Author

**Prettyg1rl19**

ABC Retailers was developed as a practical demonstration of:

* Application development
* ASP.NET Core MVC
* Cloud computing
* Azure Functions
* Azure Storage
* Authentication
* CRUD operations
* E-commerce workflows
* Responsive web development

---

## 🌸 ABC Retailers

> **Where quality meets your cart.**
