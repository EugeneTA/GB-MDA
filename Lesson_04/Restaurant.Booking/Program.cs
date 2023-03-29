using MassTransit;
using MassTransit.Audit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Restaurant.AuditLibrary;
using Restaurant.Booking.Consumer;
using Restaurant.Booking.Saga;
using Restaurant.Booking.Services;
using Restaurant.IdempotentLibrary.Models;
using Restaurant.IdempotentLibrary.Repositories;
using System.Net;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, 5001);
        });

        builder.Services.AddSingleton<IMessageAuditStore, AuditStore>();
        var serviceProvider = builder.Services.BuildServiceProvider();
        var auditStoreService = serviceProvider.GetRequiredService<IMessageAuditStore>();


        builder.Services.AddMassTransit(mt =>
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
                config.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // Включение аудита сообщений
                config.ConnectConsumeAuditObserver(auditStoreService);
                config.ConnectSendAuditObservers(auditStoreService);

                config.UseDelayedMessageScheduler();
                config.UseInMemoryOutbox();
                config.ConfigureEndpoints(context);

                config.UsePrometheusMetrics(serviceName: "booking_service");
            });
        });

        builder.Services.AddTransient<RestaurantBooking>();
        builder.Services.AddTransient<RestaurantBookingSaga>();
        builder.Services.AddSingleton<RestaurantService>();

        // Генерация запроса на бронирование стола каждые 15 секунд
        builder.Services.AddHostedService<Worker>();

        // Репозиторий полученных сообщений для запроса бронирования (тестовая реализация идемпотентности)
        builder.Services.AddSingleton<IInMemoryRepository<BookingRequestModel>, InMemoryRepository<BookingRequestModel>>();

        // Очищаем репозиторий полученных сообщений о бронировании каждые 30 секунд. 
        builder.Services.AddHostedService<ClearInMemoryRepWorker>();

        builder.Services.AddOptions<MassTransitHostOptions>()
                .Configure(options =>
                {
                    // if specified, waits until the bus is started before
                    // returning from IHostedService.StartAsync
                    // default is false
                    options.WaitUntilStarted = true;

                });

        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseRouting();

        //app.UseMetricServer(5001);
        //app.MapMetrics(pattern: "/booking_metrics");
        app.MapMetrics();
        app.MapControllers();

        app.Run();

    }
}