using System.IO;
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Calculator
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

    public class InitializedMessage{
        public int BatchId {get;set;}
        public DateTime StartDate {get;set;}
        public string AccountId {get;set;}
        public string Name {get;set;}
        public List<TradeData> Trades {get;set;}
        
        public InitializedMessage()
        {
            Trades = new List<TradeData>();
        }
    }

    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Starting Initializer");
            var factory = new ConnectionFactory() { HostName = "rabbit", Port = 5672 };
            var channel = GetChannel();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                Console.WriteLine("Message Received");
                var body = ea.Body;
                var msg = Encoding.UTF8.GetString(body);
                var message = JsonConvert.DeserializeObject<InitializedMessage>(msg);
                HandleMessage(message,channel);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: "InitializedMessage",
                            noAck: false,
                            consumer: consumer);

            System.Threading.Thread.Sleep(100000);
        }

        public static void HandleMessage(InitializedMessage message,IModel channel){
            Console.WriteLine($"Sending Calculated Message for BatchId {message.BatchId} Account {message.AccountId}-{message.Name}");
            var msg = new CalculatedMessage{
                BatchId = message.BatchId,
                AccountId = message.AccountId,
                Name = message.Name,
                AccountTotal = message.Trades.Sum(t=>t.Quantity * t.Price),
                Trades = message.Trades
            };
            var properties = channel.CreateBasicProperties();
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
            channel.BasicPublish(exchange: "",
                                routingKey: "CalculatedMessage",
                                basicProperties: properties,
                                body: body);
        }

        private static IModel GetChannel(){
             var factory = new ConnectionFactory() { HostName = "rabbit", Port = 5672 };
             var connection = factory.CreateConnection();
             var channel = connection.CreateModel();
             return channel;
        }
    }
}
