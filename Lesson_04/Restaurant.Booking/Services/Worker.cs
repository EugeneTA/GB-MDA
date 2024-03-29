﻿using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Restaurant.Booking.Models;
using Restaurant.Messages.Booking;
using System.Text;

namespace Restaurant.Booking.Services
{
    public class Worker : BackgroundService, IDisposable
    {
        private readonly IBus _bus;
        private readonly RestaurantService _restaurant;
        private readonly ILogger<Worker> _logger;

        public Worker(
            IBus bus,
            RestaurantService restaurant,
            ILogger<Worker> logger)
        {
            _bus = bus;
            _restaurant = restaurant;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.OutputEncoding = Encoding.UTF8;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.Log(LogLevel.Information, "Новый запрос на бронирование столика");

                var dateTimeNow = DateTime.Now;
                Order order = new Order(NewId.NextGuid(), NewId.NextGuid(), _restaurant.GetRandomDish());
                await _bus.Publish( 
                    (IBookingRequest) new BookingRequest(order.OrderId, order.ClientId, order.Dish, dateTimeNow, TimeSpan.FromSeconds(new Random().Next(7,15))),
                    stoppingToken
                    );

                await Task.Delay(15000, stoppingToken);
            }
        }
    }
}
