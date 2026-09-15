using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;

namespace BlobFunction
{
    internal class ProductEntity :ITableEntity
    {
        //these properties will be populated from your JSON
        [Key]
        public int Product_ID { get; set; }
        public string? Product_Name { get; set; }
        public string? Description { get; set; }
        public string? Price { get; set; }
        public string? ImageUrl { get; set; } // very baie important for the adding of the picture

        // ---Required Table storage properties ---
        //PartitionKey is used to group related entities
        //We will set this to "People" in our function
        public string? PartitionKey { get; set; } = "Products";
        //RowKey is the unique ID for an entity within a partition
        //We will generate a new GUID for this in our function

        public string? RowKey { get; set; }
        //Required by the ITableEntity Interface

        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
