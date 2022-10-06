using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;  
namespace MessageReader
{
   public class Program
   {
      private const string storageConnectionString = "Endpoint=sb://sbnamespacesatish.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=5NN3/yybHGX4hiZMl8ivN/0IQZvZHuBFnMjeyV8L3kc=";
      static string queueName = "messagequeue";
      static ServiceBusClient client = default!;

      static ServiceBusProcessor processor = default!;
      static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");
            await args.CompleteMessageAsync(args.Message);
        }
        
        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        static async Task Main(string[] args)
        {
            client = new ServiceBusClient(storageConnectionString);
            processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
            try
            {
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;
                await processor.StartProcessingAsync();
                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();
                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            finally
            {
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }

            
        }
   }
}