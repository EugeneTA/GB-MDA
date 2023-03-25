using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Booking.Consumer;
using Restaurant.Booking.Saga;
using Restaurant.Booking.Services;
using Restaurant.IdempotentLibrary.Models;
using Restaurant.IdempotentLibrary.Repositories;

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
                    mt.AddConsumer<BookingRequestConsumer>(c =>
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

                    mt.AddConsumer<BookingRequestFaultConsumer>(c =>
                    {
                        //c.UseScheduledRedelivery(r =>
                        //{
                        //    r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15));
                        //});
                        //c.UseMessageRetry(r =>
                        //{
                        //    r.Incremental(2, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(2));
                        //});
                    });
                    //.Endpoint(e => e.Temporary = true);

                    mt.AddConsumer<WaitingClientConsumer>(c =>
                    {
                        c.UseScheduledRedelivery(r =>
                        {
                            r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
                        });
                        c.UseMessageRetry(r =>
                        {
                            r.Incremental(1, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
                        });
                    });
                    //.Endpoint(e => e.Temporary = true);

                    mt.AddSagaStateMachine<RestaurantBookingSaga, RestaurantBooking>()
                    //.Endpoint(e => e.Temporary = true)
                    .InMemoryRepository();

                    mt.AddDelayedMessageScheduler();

                    mt.UsingRabbitMq((context, config) =>
                    {
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
                
                // Генерация запроса на бронирование стола каждые 15 секунд
                services.AddHostedService<Worker>();

                // Репозиторий полученных сообщений для запроса бронирования (тестовая реализация идемпотентности)
                services.AddSingleton<IInMemoryRepository<BookingRequestModel>, InMemoryRepository<BookingRequestModel>>();

                // Очищаем репозиторий полученных сообщений о бронировании каждые 30 секунд. 
                services.AddHostedService<ClearInMemoryRepWorker>();

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