using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.MultiBus;
using Restaurant.Notification.Consumers;

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
                    services.AddMassTransit(mt =>
                    {
                        mt.AddConsumer<NotifyConsumer>().Endpoint(e => e.Temporary = true);

                        mt.UsingRabbitMq((context, config) => 
                        {
                            config.Host("localhost", "/", h => {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            config.ConfigureEndpoints(context);
                        });
                    });

                    services.AddSingleton<Notifier>();

                    services.AddOptions<MassTransitHostOptions>()
                            .Configure(options =>
                            {
                                // if specified, waits until the bus is started before
                                // returning from IHostedService.StartAsync
                                // default is false
                                options.WaitUntilStarted = true;

                            });

                }).Build().Run();
        }
    }
}