using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
namespace MessagePublisher
{
    public class Program
    {
        private const string storageConnectionString = "Endpoint=sb://sbnamespacesatish.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=5NN3/yybHGX4hiZMl8ivN/0IQZvZHuBFnMjeyV8L3kc=";
        private const string queueName = "messagequeue";
        private const int numOfMessages = 3;
        static ServiceBusClient client = default!;
        static ServiceBusSender sender = default!;
        public static async Task Main(string[] args)
        {
            client = new ServiceBusClient(storageConnectionString);
            sender = client.CreateSender(queueName);  
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            for (int i = 1; i <= numOfMessages; i++)
            {
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
                {
                    throw new Exception($"The message {i} is too large to fit in the batch.");
                }
            }
            try
            {
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");
            }
            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}