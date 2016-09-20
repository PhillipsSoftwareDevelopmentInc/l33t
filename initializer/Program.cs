using System.IO;
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;

namespace Initializer
{
    public class StartBatchCommand {
        public int BatchId {get;set;}
        public DateTime StartDate {get;set;}
    }
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Starting Initializer");
            var factory = new ConnectionFactory() { HostName = "rabbit", Port = 5672 };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "StartBatchCommand",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    Console.WriteLine("Message Received");
                    var body = ea.Body;
                    var msg = Encoding.UTF8.GetString(body);
                    var message = JsonConvert.DeserializeObject<StartBatchCommand>(msg);
                    Console.WriteLine($"Start Batch Processed {message.BatchId} : {message.StartDate}");
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "StartBatchCommand",
                                 noAck: false,
                                 consumer: consumer);

            }
            Console.ReadLine();
        }
    }
}