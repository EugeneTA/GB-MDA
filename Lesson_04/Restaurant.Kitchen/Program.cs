using MassTransit;
using MassTransit.Audit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Restaurant.AuditLibrary;
using Restaurant.Booking.Services;
using Restaurant.IdempotentLibrary.Models;
using Restaurant.IdempotentLibrary.Repositories;
using Restaurant.Kitchen.Consumers;
using Restaurant.Kitchen.Services;
using System.Net;

namespace Restaurant.Kitchen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, 5002);
            });

            // Регистрация сервиса аудита сообщений
            builder.Services.AddSingleton<IMessageAuditStore, AuditStore>();
            var serviceProvider = builder.Services.BuildServiceProvider();
            var auditStoreService = serviceProvider.GetRequiredService<IMessageAuditStore>();

            builder.Services.AddMassTransit(mt =>
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
                    config.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    // Включение аудита сообщений
                    config.ConnectConsumeAuditObserver(auditStoreService);
                    config.ConnectSendAuditObservers(auditStoreService);

                    config.UseInMemoryOutbox();
                    config.ConfigureEndpoints(context);

                    config.UsePrometheusMetrics(serviceName: "kitchen_service");
                });
            });

            builder.Services.AddSingleton<Manager>();

            // Репозиторий полученных сообщений для запроса бронирования (тестовая реализация идемпотентности)
            builder.Services.AddSingleton<IInMemoryRepository<BookingRequestModel>, InMemoryRepository<BookingRequestModel>>();

            // Очищаем репозиторий полученных сообщений о бронировании каждые 30 секунд. 
            builder.Services.AddHostedService<ClearInMemoryRepWorker>();

            //builder.Services.AddHostedService<Worker>();

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

            //app.UseMetricServer(5002);
            //app.MapMetrics(pattern: "/kitchen_metrics");
            app.MapMetrics();
            app.MapControllers();

            app.Run();
        }
    }
}