using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Azure;
using Azure.Data.Tables;

namespace ST10438767_CLDV6212.Models
{
    public class Order : ITableEntity
    {
        [Key]
        [JsonPropertyName("id")]
        public int Order_Id { get; set; }

        //ITableEntity properties
        [JsonPropertyName("partitionkey")]
        public string? PartitionKey { get; set; }

        [JsonPropertyName("rowkey")]
        public string? RowKey { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset? Timestamp { get; set; }

        [JsonPropertyName("etag")]
        public ETag ETag { get; set; }

        //Error handling properties
        [Required(ErrorMessage = "Please select a customer")]
        [JsonPropertyName("customerid")]
        public int Customer_Id { get; set; } //FK for the customer who is ordering

        [Required(ErrorMessage = "Please select a product")]
        [JsonPropertyName("prouctid")]
        public int Product_ID { get; set; } //FK for the product the customer is gonna order

        [Required(ErrorMessage = "Please select a quantity")]
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; } //Quantity of the product the customer is ordering

        [JsonPropertyName("date")]
        [Required(ErrorMessage = "Please select the date of delivery")]
        public DateTime Delivery_Date { get; set; } //Date of the order

        [JsonPropertyName("location")]
        [Required(ErrorMessage = "Please select the delivery address")]
        public string? Delivery_Location { get; set; } //Location of the delivery

        [JsonPropertyName("status")]
        [Required(ErrorMessage = "Please select the order status")]
        public string? Order_Status { get; set; } //Location of the delivery

        /*
         IIEVC School of Computer Science, 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 3: Never Lose Data Again with Queue Storage!
         [video online] Available at:<https://www.youtube.com/watch?v=VbZ3Pi63yEc&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=5> [Accessed 17 August 2025]. 
        */

    }
}
