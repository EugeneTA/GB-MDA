﻿using Microsoft.Extensions.Hosting;
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
                        mt.AddConsumer<NotifyConsumer>(c =>
                        {
                            // Повторная отправка сообщений второго уровня
                            c.UseScheduledRedelivery(r =>
                            {
                                r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                            });
                            // Повторная отправка при кратковременном отсутствии связи
                            c.UseMessageRetry(r =>
                            {
                                r.Incremental(1, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
                            });
                        });
                        //.Endpoint(e => e.Temporary = true);

                        mt.UsingRabbitMq((context, config) => 
                        {
                            config.Host("localhost", "/", h => {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            config.UseInMemoryOutbox();
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