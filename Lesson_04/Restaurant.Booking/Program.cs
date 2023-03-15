using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Booking.Consumer;
using Restaurant.Booking.Saga;
using Restaurant.Booking.Services;

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
                    mt.AddConsumer<BookingRequestConsumer>().Endpoint(e => e.Temporary = true);
                    mt.AddConsumer<BookingRequestFaultConsumer>().Endpoint(e => e.Temporary = true);
                    mt.AddConsumer<WaitingClientConsumer>().Endpoint(e => e.Temporary = true);

                    mt.AddSagaStateMachine<RestaurantBookingSaga, RestaurantBooking>()
                    .Endpoint(e => e.Temporary = true)
                    .InMemoryRepository();

                    mt.AddDelayedMessageScheduler();

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

                        config.UseDelayedMessageScheduler();
                        config.UseInMemoryOutbox();
                        config.ConfigureEndpoints(context);
                    });
                });

                services.AddTransient<RestaurantBooking>();
                services.AddTransient<RestaurantBookingSaga>();
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