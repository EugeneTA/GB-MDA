using Messaging;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RestaurantBooking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Notification
{
    public class Worker : BackgroundService, IDisposable
    {
        private readonly Consumer _consumer;

        public Worker()
        {
            ConnectionSettings connectionSettings = new ConnectionSettings();

            _consumer = new Consumer(
                connectionSettings.QueueName,
                connectionSettings.HostName,
                connectionSettings.VirtualHostName,
                connectionSettings.UserName,
                connectionSettings.Password
                );
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Recieve("notificatons", ExchangeType.Fanout, (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Recieved: {message}");
            });
        }
    }
}
