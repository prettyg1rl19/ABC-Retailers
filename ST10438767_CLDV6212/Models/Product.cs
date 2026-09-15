using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Azure;
using Azure.Data.Tables;

namespace ST10438767_CLDV6212.Models
{
    //Equivalent to "AddPersonWithImage"
    public class Product : ITableEntity
    {
        //? = nullable reference type, allows the property to be null
        //This is useful for properties that may not always have a value, such as optional fields in a database.
        //And this also prevents pesky errors

        [Key]
        [JsonPropertyName("id")]
        public int Product_ID { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string? Product_Name { get; set; }

        [Required]
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [Required]
        [JsonPropertyName("price")]
        public int Price { get; set; }

        //Part 2 1st change to original code.
        [Display(Name = "Product Image")]
        public string? ImageUrl { get; set; }

        //Part 2 1st change to original code.
        [NotMapped]
        [Display(Name = "Product Image")]
        public IFormFile? ImageURL { get; set; }

        //ITableEntity implementation
        //These variales are required for Azure Table Storage to work properly
        [JsonPropertyName("partitionkey")]
        public string? PartitionKey { get; set; }

        [JsonPropertyName("rowkey")]
        public string? RowKey { get; set; }

        [NotMapped]
        [JsonPropertyName("etag")]
        public ETag ETag { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset? Timestamp { get; set; }

        /*
        IIEVC School of Computer Science, 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 2: Adding Image Uploads with Blob Storage!
        [video online] Available at:<https://www.youtube.com/watch?v=CuszKqZvRuM&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=4> [Accessed 16 August 2025]. 
        */
    }
}
