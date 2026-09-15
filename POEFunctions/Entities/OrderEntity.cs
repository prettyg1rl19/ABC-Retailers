using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;

namespace POEFunctions.Entities
{
    public class OrderEntity : ITableEntity
    {

        [Key]
        public int Order_Id { get; set; }

        //ITableEntity properties
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        //Error handling properties
        [Required(ErrorMessage = "Please select a customer")]
        public int Customer_Id { get; set; } //FK for the customer who is ordering

        [Required(ErrorMessage = "Please select a product")]
        public int Product_ID { get; set; } //FK for the product the customer is gonna order

        [Required(ErrorMessage = "Please select a quantity")]
        public int Quantity { get; set; } //Quantity of the product the customer is ordering

        [Required(ErrorMessage = "Please select the date of delivery")]
        public DateTime Delivery_Date { get; set; } //Date of the order

        [Required(ErrorMessage = "Please select the delivery address")]
        public string? Delivery_Location { get; set; } //Location of the delivery

        [Required(ErrorMessage = "Please select the order status")]
        public string? Order_Status { get; set; } //Location of the delivery
    }
}
/*
 IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 1 Getting the basics out the way HTTP Trigger
[video online] Available at:<https://www.youtube.com/watch?v=l7s5u-QzYe8&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=7> [Accessed 24 September 2025]. 

IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 2 Azure functions and queues triggers
[video online] Available at:<https://www.youtube.com/watch?v=zP4umzRCsTM&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=8> [Accessed 25 September 2025].

*/