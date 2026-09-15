using System.ComponentModel.DataAnnotations;

namespace ST10438767_CLDV6212.Models
{
    public class CartItem
    {
        [Key]
        public Guid CartItemGuid { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCategory { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string DeliveryAddress { get; set; }

        /*
        Reitan, E., 2025. Shopping Cart. [Online] Available at: 
        < https://learn.microsoft.com/en-us/aspnet/web-forms/overview/getting-started/getting-started-with-aspnet-45-web-forms/shopping-cart > [Accessed 12 November 2025].
         */
    }


}
