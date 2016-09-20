using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace Receiptze
{
    public class StartBatchCommand {
        public int BatchId {get;set;}
        public DateTime StartDate {get;set;}
    }
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index() => View();

        [HttpGet("/Test")]
        public IActionResult Test()
        {
          int n = 1;
          while (n < 6)
          {
            try{
                var factory = new ConnectionFactory() { HostName = "rabbit", Port = 5672 };
                using(var connection = factory.CreateConnection())
                using(var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                    string message = "Hello World!";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                        routingKey: "hello",
                                        basicProperties: null,
                                        body: body);

                }
                n = 7;
                return View("Healthy");
            }catch(Exception e){
                ViewBag.Error = e.Message;
                n++;
                return View("NotHealthy");
            }
          }
          return View("NotHealthy");

        }

        [HttpGet("/Run")]
        public IActionResult Run(){
            try{
                var command = new StartBatchCommand{BatchId=DateTime.Now.Second,StartDate=DateTime.Now};
                var factory = new ConnectionFactory() { HostName = "rabbit", Port = 5672 };
                using(var connection = factory.CreateConnection())
                using(var channel = connection.CreateModel())
                {
                    var properties = channel.CreateBasicProperties();
                    channel.QueueDeclare(queue: "StartBatchCommand",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
                    channel.QueueDeclare(queue: "Initialized",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
                    channel.QueueDeclare(queue: "Calced",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
                    channel.QueueDeclare(queue: "Pdfgen",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(command));
                    channel.BasicPublish(exchange: "",
                                        routingKey: "StartBatchCommand",
                                        basicProperties: properties,
                                        body: body);
                    Console.WriteLine("Sent StartBatchCommand");
                }
                return View("Success");
            }catch(Exception e){
                Console.WriteLine($"Error sending StartBatchCommand {e.Message}");
                return View("Error");
            }

        }
    }
}
