using System.IO;
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using Amazon.S3;
using Amazon.S3.Model;
using System.Text;
using System.Collections.Generic;

namespace PdfGen
{
    public class CalculatedMessage {
        public int BatchId {get;set;}
        public DateTime StartDate {get;set;}
        public string AccountId {get;set;}
        public string Name {get;set;}
        public List<TradeData> Trades {get;set;}
        public decimal AccountTotal {get;set;}
        public CalculatedMessage ()
        {
            Trades = new List<TradeData>();
        }
    }
    public class TradeData {
        public int Quantity {get;set;}
        public decimal Price {get;set;}
    }

    public class PdfGeneratedMessage{
        public int BatchId {get;set;}
        public string AccountId {get;set;}
        public decimal AccountTotal {get;set;}
        public DateTime StartDate {get;set;}
    }

    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Starting PdfGen");
            var factory = new ConnectionFactory() { HostName = "rabbit", Port = 5672 };
            var channel = GetChannel();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                Console.WriteLine("Message Received");
                var body = ea.Body;
                var msg = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject<CalculatedMessage>(msg);
                HandleMessage(message,channel);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: "CalculatedMessage",
                            noAck: false,
                            consumer: consumer);

            Console.ReadLine();
        }

        public static void HandleMessage(CalculatedMessage message,IModel channel){
            Console.WriteLine($"Generating PDF for BatchId {message.BatchId} Account {message.AccountId}-{message.Name}");
            var id = Guid.NewGuid();
            var properties = channel.CreateBasicProperties();
            var msg = new PdfGeneratedMessage{BatchId = message.BatchId,AccountId=message.AccountId,StartDate=DateTime.Now,AccountTotal=message.AccountTotal};
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
            channel.BasicPublish(exchange: "",
                                routingKey: "PdfGeneratedMessage",
                                basicProperties: properties,
                                body: body);
            Console.WriteLine($"Document {id}.pdf created"); 
        }

        private static IModel GetChannel(){
             var factory = new ConnectionFactory() { HostName = "rabbit", Port = 5672 };
             var connection = factory.CreateConnection();
             var channel = connection.CreateModel();
             return channel;
        }
    }
}
