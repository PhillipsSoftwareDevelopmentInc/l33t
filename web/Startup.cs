using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace HelloMvc
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(LogLevel.Debug);

            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
              int n = 1;
              var connected = false;
              while (n < 6)
              {
                try{
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
                        channel.QueueDeclare(queue: "InitializedMessage",
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);
                        channel.QueueDeclare(queue: "CalculatedMessage",
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);
                        channel.QueueDeclare(queue: "Pdfgen",
                                            durable: false,
                                            exclusive: false,
                                            autoDelete: false,
                                            arguments: null);
                        n = 7;
                        connected = true;
                      }
                    }catch(Exception e){
                          n++;
                          Console.WriteLine($"{e.Message}");
                    }
                }
            if (!connected)
            {
              throw new Exception("Shiz failed");
            }

            app.UseMvc();
        }
    }
}
