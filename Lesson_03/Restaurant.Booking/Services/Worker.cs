using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Restaurant.Booking.Config;
using Restaurant.Booking.Models;
using Restaurant.Messages;
using System.Text;
using State = Restaurant.Booking.Models.State;

namespace Restaurant.Booking.Services
{
    public class Worker : BackgroundService, IDisposable
    {
        private readonly IBus _bus;
        private readonly RestaurantService _restaurant;
        private readonly IOptions<RabbitMQConfig> _cfg;

        public Worker(
            IBus bus,
            RestaurantService restaurant,
            IOptions<RabbitMQConfig> configuration)
        {
            _bus = bus;
            _restaurant = restaurant;
            _cfg = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.OutputEncoding = Encoding.UTF8;
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken);
                Console.WriteLine("Добрый день! Желаете забронировать столик?");
                var dateTimeNow = DateTime.Now;

                Order order = new Order(NewId.NextGuid(), NewId.NextGuid(), _restaurant.GetRandomDish());

                var result = await _restaurant.BookFreeTableAsync(1, order);

                if (result != null) 
                {
                    Console.WriteLine($"Заказ {order.OrderId}. Бронируем столик номер {result.Id}. Предзаказ {result.Order.Dish}");
                    await _bus.Publish(
                            new TableBooked(result.Order.OrderId, result.Order.ClientId, result.State == State.Booked ? true : false, result.Order.Dish),
                            context => context.Durable = false,
                            stoppingToken
                            );
                }
                else
                {
                    Console.WriteLine($"Заказ {order.OrderId}. Все столы забронированы.");
                    await _bus.Publish(
                            new TableBooked(order.OrderId, order.ClientId, false),
                            context => context.Durable = false,
                            stoppingToken
                            );
                }
            }
        }
    }
}
