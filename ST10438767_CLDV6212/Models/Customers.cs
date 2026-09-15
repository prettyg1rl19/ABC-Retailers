using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10438767_CLDV6212.Models
{
    [Table("Customers")]
    public class Customers
    {
        [Key]
        public int customerId { get; set; }
        public string? customerName { get; set; }
        public string? customerEmail { get; set; }
        public string? customerAddress { get; set; }
        public string? PasswordHash { get; set; }
        public string Role { get; set; } = "Customer";
    }
}
