using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ST10438767_CLDV6212.Models
{
    public class Products
    {
        [Key]
        public int productId { get; set; }
        public string? productName { get; set; }
        public string? description { get; set; }
        public int price { get; set; }
        public string? imageUrl { get; set; }
    }
}
