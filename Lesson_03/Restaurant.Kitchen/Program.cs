﻿using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Kitchen.Consumers;
using Restaurant.Kitchen.Services;

namespace Restaurant.Kitchen
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
                        mt.AddConsumer<KitchenBookingRequestConsumer>().Endpoint(e => e.Temporary = true);


                        mt.UsingRabbitMq((context, config) =>
                        {
                            //config.Host("cow.rmq2.cloudamqp.com", "srgicxjt", h => {
                            //    h.Username("srgicxjt");
                            //    h.Password("ztUKEjNXQxDlxha5npbLMSKc-Ecrf_gx");
                            //});

                            config.Host("localhost", "/", h => {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            config.ConfigureEndpoints(context);
                        });
                    });

                    services.AddSingleton<Manager>();
                    //services.AddHostedService<Worker>();
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