using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;

namespace Table_Function
{
    internal class CustomerEntity : ITableEntity
    {
        public int Customer_Id { get; set; }
        public string? Customer_Name { get; set; }
        public string? Customer_Email { get; set; }
        public string? Customer_Address { get; set; }

        //ITableEntity properties & Implementation
        //These variales are required for Azure Table Storage to work properly
        public string PartitionKey { get; set; } = "Customers";
        public string RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
