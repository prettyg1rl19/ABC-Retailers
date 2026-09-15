using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;

namespace POEFunctions
{
    public class CustomerEntity : ITableEntity
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
/*
 IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 1 Getting the basics out the way HTTP Trigger
[video online] Available at:<https://www.youtube.com/watch?v=l7s5u-QzYe8&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=7> [Accessed 24 September 2025]. 

IIE Emeris School of Computer Science , 2025. CLDV6212 Azure functions part 2 Azure functions and queues triggers
[video online] Available at:<https://www.youtube.com/watch?v=zP4umzRCsTM&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=8> [Accessed 25 September 2025].
*/
