using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Messaging;
using RabbitMQ.Client;

namespace Restaurant.Notification
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                }).Build().Run();
        }
    }
}