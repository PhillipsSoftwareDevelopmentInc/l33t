using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using RabbitMQ.Client;

namespace Receiptze
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index() => View();

        [HttpGet("/Test")]
        public IActionResult Test()
        {
            try{
                var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
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
                return View("Healthy");
            }catch(Exception e){
                ViewBag.Error = e.Message;
                return View("NotHealthy");
            }
        }
    }
}