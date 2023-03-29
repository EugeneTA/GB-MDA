using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.MultiBus;
using Restaurant.Notification.Consumers;
using Restaurant.IdempotentLibrary.Repositories;
using Restaurant.IdempotentLibrary.Models;
using MassTransit.Audit;
using Restaurant.AuditLibrary;
using Microsoft.AspNetCore.Builder;
using Prometheus;
using Microsoft.AspNetCore.Hosting;
using System.Net;

namespace Restaurant.Notification
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, 5003);
            });

            // Регистрация сервиса аудита сообщений
            builder.Services.AddSingleton<IMessageAuditStore, AuditStore>();
            var serviceProvider = builder.Services.BuildServiceProvider();
            var auditStoreService = serviceProvider.GetRequiredService<IMessageAuditStore>();

            builder.Services.AddMassTransit(mt =>
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

                    config.UsePrometheusMetrics(serviceName: "notification_service");

                });
            });

            // Сервис уведомления пользователя
            builder.Services.AddSingleton<Notifier>();

            // Репозиторий полученных сообщений уведомления пользователя (тестовая реализация идемпотентности)
            builder.Services.AddSingleton<IInMemoryRepository<NotifyModel>, InMemoryRepository<NotifyModel>>();

            // Очищаем репозиторий полученных сообщений уведомления пользователя каждые 30 секунд. 
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

            //app.UseMetricServer(5003);
            //app.MapMetrics(pattern: "/notification_metrics");
            app.MapMetrics();
            app.MapControllers();

            app.Run();

        }
    }
}