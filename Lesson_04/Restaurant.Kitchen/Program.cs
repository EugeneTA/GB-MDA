using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Booking.Services;
using Restaurant.IdempotentLibrary.Models;
using Restaurant.IdempotentLibrary.Repositories;
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
                        mt.AddConsumer<RestaurantBookingRequestConsumer>(c =>
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

                    services.AddSingleton<Manager>();

                    // Репозиторий полученных сообщений для запроса бронирования (тестовая реализация идемпотентности)
                    services.AddSingleton<IInMemoryRepository<BookingRequestModel>, InMemoryRepository<BookingRequestModel>>();

                    // Очищаем репозиторий полученных сообщений о бронировании каждые 30 секунд. 
                    services.AddHostedService<ClearInMemoryRepWorker>();
                    
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