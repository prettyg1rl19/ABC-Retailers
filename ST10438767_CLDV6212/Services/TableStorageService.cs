using Azure;
using Azure.Data.Tables;
using ST10438767_CLDV6212.Models;
namespace ST10438767_CLDV6212.Services
{
    public class TableStorageService
    {
        //Gonna be used to connect the IDE to the table we're connecting to
        private readonly TableClient _customerTableClient; //for the customer Table
        private readonly TableClient _productTableClient; //for the product Table
        private readonly TableClient _orderTableClient; //and for the order Table


        public TableStorageService(string connectionString)
        {
            _customerTableClient = new TableClient(connectionString, "customer"); //table name must match the one in azure
            _productTableClient = new TableClient(connectionString, "product"); //same goes for this one
            _orderTableClient = new TableClient(connectionString, "order"); //and let us not forget this one!
        }



        //All Customer methods!
        //This method is to gather a list of allll the customers we have.
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            //this is gonnna loop and get us a niftly little list of all the customers in the system.
            var customers = new List<Customer>();

            //So it's gonna loop through that query and build a list oof all our customers - added by the adminn of ABC Retail (us)
            //The it's gonna be passed back into the list and boom! A list
            await foreach (var customer in _customerTableClient.QueryAsync<Customer>())
            {
                customers.Add(customer);
            }

            return customers;
        }

        //This method s to add the customer to the tables
        public async Task AddCustomerAsync(Customer customer)
        {
            //checks if the partitionkey and rowkey is set
            if (string.IsNullOrEmpty(customer.PartitionKey) || string.IsNullOrEmpty(customer.RowKey))
            {
                //if not, this error message will be thrown
                throw new ArgumentException("PartitionKey and RowKey must be set.");
            }

            //If it has been set, then it will try adding the customer to the Table.
            try
            {
                await _customerTableClient.AddEntityAsync(customer);
                //If it works, it gets added successfully :)
            }

            //If it doesn't work, you then get this lovely little error message.
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException("Error in adding the selected enity to the Table Storage", ex);
            }
        }

        //This onne is quite simple, it simplly deletes the entity.
        public async Task DeleteCustomerAsync(string partitionKey, string rowKey)
        {
            await _customerTableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        //This onne is quite simple, it simplly deletes the entity.
        public async Task EditCustomerAsync(string partitionKey, string rowKey, Customer customer)
        {
            await _customerTableClient.UpdateEntityAsync(customer, ETag.All, TableUpdateMode.Replace);
        }

        public async Task<int> GenerateNextCustomerIdAsync()
        {
            var customer = await GetAllCustomersAsync();
            if (customer == null || customer.Count == 0)
                return 1;

            return customer.Max(p => p.Customer_Id) + 1;
        }



        //All Product Methods!
        //Again, this bad boy is to gather us a list of allll of the products
        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = new List<Product>();
            await foreach (var product in _productTableClient.QueryAsync<Product>())
            {
                products.Add(product);
            }
            return products;
        }

        public async Task AddProductAsync(Product product)
        {
            if (string.IsNullOrEmpty(product.PartitionKey) || string.IsNullOrEmpty(product.RowKey))
            {
                throw new ArgumentException("PartitionKey and RowKey must be set.");
            }
            try
            {
                await _productTableClient.AddEntityAsync(product);
            }
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException($"Error adding entity to table storage", ex);
            }
        }

        public async Task DeleteProductAsync(string partitionKey, string rowKey)
        {
            await _productTableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        //This onne is quite simple, it simplly deletes the entity.
        public async Task EditProductAsync(string partitionKey, string rowKey, Product product)
        {
            await _productTableClient.UpdateEntityAsync(product, ETag.All, TableUpdateMode.Replace);
        }

        public async Task<int> GenerateNextProductIdAsync()
        {
            var products = await GetAllProductsAsync();
            if (products == null || products.Count == 0)
                return 1;

            return products.Max(p => p.Product_ID) + 1;
        }



        //All of the Orders Methods
        //Get all of the orders andshow them in a LIST
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            //this is gonnna loop and get us a niftly little list of all the products in the system.
            var orders = new List<Order>();

            //So it's gonna loop through that query and build a list oof all our products - added by the adminn of ABC Retail (us)
            //The it's gonna be passed back into the list and boom! A list
            await foreach (var order in _orderTableClient.QueryAsync<Order>())
            {
                orders.Add(order);
            }
            return orders;
        }

        //This lil method is to add a new one to the LIST
        public async Task AddOrderAsync(Order order)
        {
            //checks if the partitionkey and rowkey is set
            if (string.IsNullOrEmpty(order.PartitionKey) || string.IsNullOrEmpty(order.RowKey))
            {
                //if not, this error message will be thrown
                throw new ArgumentException("PartitionKey and RowKey must be set.");
            }

            //If it has been set, then it will try adding the product to the Table.
            try
            {
                await _orderTableClient.AddEntityAsync(order);
                //If it works, it gets added successfully :)
            }

            //If it doesn't work, you then get this lovely little error message.
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException("Error in adding the selected enity to the Table Storage", ex);
            }
        }

        //This onne is quite simple, it simplly deletes the entity.
        public async Task DeleteOrderAsync(string partitionKey, string rowKey)
        {
            await _orderTableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        //This one is quite simple, it simplly edits the entity.
        public async Task EditOrderAsync(string partitionKey, string rowKey, Order order)
        {
            await _orderTableClient.UpdateEntityAsync(order, ETag.All, TableUpdateMode.Replace);
        }
        /*
        Microsoft, 2025. TableUpdateMode Enum
        [online] Available at: <https://learn.microsoft.com/en-us/dotnet/api/azure.data.tables.tableupdatemode?view=azure-dotnet> [Accessed 21 August 2025].
        */

        public async Task<int> GenerateNextOrderIdAsync()
        {
            var order = await GetAllOrdersAsync();
            if (order == null || order.Count == 0)
                return 1;

            return order.Max(p => p.Customer_Id) + 1;
        }
    }

    /*
     IIEVC School of Computer Science, 2025. CLDV6212 Building a Modern Web App with Azure Table Storage & ASP.NET Core MVC - Part 1
     [video online] Available at:<https://www.youtube.com/watch?v=Txp7VYUMBGQ&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=3> [Accessed 16 August 2025].
     
     IIEVC School of Computer Science, 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 2: Adding Image Uploads with Blob Storage!
     [video online] Available at:<https://www.youtube.com/watch?v=CuszKqZvRuM&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=4> [Accessed 16 August 2025]. 

     IIEVC School of Computer Science, 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 3: Never Lose Data Again with Queue Storage!
     [video online] Available at:<https://www.youtube.com/watch?v=VbZ3Pi63yEc&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=5> [Accessed 17 August 2025]. 
     */
}
