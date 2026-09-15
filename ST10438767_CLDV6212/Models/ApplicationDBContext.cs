using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;

namespace ST10438767_CLDV6212.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<CartItem> ShoppingCartItem { get; set; }
    }

    //Mrzygłód, K., 2022. Azure for Developers.
    //This class represents the database context for the application, enabling interaction with the database using Entity Framework Core.
}

