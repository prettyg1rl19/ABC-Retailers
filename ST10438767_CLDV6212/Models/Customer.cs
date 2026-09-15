using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Azure;
using Azure.Data.Tables;

namespace ST10438767_CLDV6212.Models
{
    //Equivalent to "Person"
    //2nd changes made to part 1 code
    public class Customer : ITableEntity
    {
        //These attributes map the json properties (e.g., "name")
        //to our C# property (e.g., "Name").
        [Key]
        [JsonPropertyName("id")]
        public int Customer_Id { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string? Customer_Name { get; set; }

        [Required]
        [JsonPropertyName("email")]
        public string? Customer_Email { get; set; }

        [Required]
        [JsonPropertyName("address")]
        public string? Customer_Address { get; set; }

        //ITableEntity properties & Implementation
        //These variales are required for Azure Table Storage to work properly
        [JsonPropertyName("partitionkey")]
        public string PartitionKey { get; set; }

        [JsonPropertyName("rowkey")]
        public string RowKey { get; set; }

        [NotMapped]
        [JsonPropertyName("etag")]
        public ETag ETag { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTimeOffset? Timestamp { get; set; }
    }
}
