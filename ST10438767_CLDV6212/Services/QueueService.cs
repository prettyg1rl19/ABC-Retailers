using Azure.Storage.Queues;
namespace ST10438767_CLDV6212.Services
{
    public class QueueService
    {
        private readonly QueueClient _queueClient;

        public QueueService(string connectionString, string queueName)
        {
            _queueClient = new QueueClient(connectionString, queueName);
        }

        public async Task SendMessage(string message)
        {
            await _queueClient.SendMessageAsync(message);
        }
    }

    /*IIEVC School of Computer Science, 2025. CLDV6212 ASP.NET MVC & Azure Series - Part 3: Never Lose Data Again with Queue Storage!
[video online] Available at:<https://www.youtube.com/watch?v=VbZ3Pi63yEc&list=PL480DYS-b_kcZiyuCyHolh6Nad8J_Xnk7&index=5> [Accessed 17 August 2025]. */
}
