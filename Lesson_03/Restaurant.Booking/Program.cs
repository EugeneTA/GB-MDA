using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Booking.Consumer;
using Restaurant.Booking.Services;
using Restaurant.Notification.Consumers;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMassTransit(mt =>
                {
                    mt.AddConsumer<BookingKitchenAccidentConsumer>();
                    mt.AddConsumer<TableBookedConsumer>();

                    mt.UsingRabbitMq((context, config) =>
                    {
                        config.Host("cow.rmq2.cloudamqp.com", "srgicxjt", h => {
                            h.Username("srgicxjt");
                            h.Password("ztUKEjNXQxDlxha5npbLMSKc-Ecrf_gx");
                        });
                        config.ConfigureEndpoints(context);
                    });
                });

                services.AddSingleton<RestaurantService>();
                services.AddHostedService<Worker>();

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