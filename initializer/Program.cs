using System.IO;
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Initializer
{
    public class StartBatchCommand {
        public int BatchId {get;set;}
        public DateTime StartDate {get;set;}
    }
    public class InitializedMessage {
        public int BatchId {get;set;}
        public DateTime StartDate {get;set;}
        public string AccountId {get;set;}
        public string Name {get;set;}
        public List<TradeData> Trades {get;set;}
        public InitializedMessage ()
        {
            Trades = new List<TradeData>();
        }
    }
    public class AccountData {
        public DateTime StartDate {get;set;}
        public string AccountId {get;set;}
        public string Name {get;set;}
        public List<TradeData> Trades {get;set;}
    }
    public class TradeData {
        public int Quantity {get;set;}
        public decimal Price {get;set;}
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
                var message = JsonConvert.DeserializeObject<StartBatchCommand>(msg);
                HandleMessage(message,channel);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: "StartBatchCommand",
                            noAck: false,
                            consumer: consumer);

            Console.ReadLine();
        }

        private static IModel GetChannel(){
             var factory = new ConnectionFactory() { HostName = "rabbit", Port = 5672 };
             var connection = factory.CreateConnection();
             var channel = connection.CreateModel();
             return channel;
        }

        public static void HandleMessage(StartBatchCommand message,IModel channel){

            Console.WriteLine($"Start Batch Command Processed {message.BatchId} : {message.StartDate}");
            var dummyData = new List<AccountData>{
                new AccountData{
                    AccountId = "G1000",
                    Name = "Super Duper Farmer",
                    Trades = new List<TradeData>()
                },
                new AccountData{
                    AccountId = "1010",
                    Name = "Bernie Madoff",
                    Trades = new List<TradeData>{
                        new TradeData{Quantity = 5,Price = .50m},
                        new TradeData{Quantity = 3,Price = .25m},
                        new TradeData{Quantity = 5,Price = 1.73m},
                    }
                },
                new AccountData{
                    AccountId = "9999",
                    Name = "Lame Farmer",
                    Trades = new List<TradeData>()
                },
            };
            var properties = channel.CreateBasicProperties();
            foreach(var account in dummyData){
                Console.WriteLine($"Sending Initialized Message for BatchId {message.BatchId} Account {account.AccountId}-{account.Name}");
                var msg = new InitializedMessage{
                    BatchId = message.BatchId,
                    StartDate = message.StartDate,
                    AccountId = account.AccountId,
                    Name = account.Name,
                    Trades = account.Trades
                };
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
                channel.BasicPublish(exchange: "",
                                    routingKey: "InitializedMessage",
                                    basicProperties: properties,
                                    body: body);
            }
        }
    }
    
}
