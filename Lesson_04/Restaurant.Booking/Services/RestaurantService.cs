﻿using MassTransit;
using Restaurant.Booking.Models;
using Restaurant.Messages;
using State = Restaurant.Booking.Models.State;

namespace Restaurant.Booking.Services
{
    public class RestaurantService
    {
        private readonly IBus _bus;
        private readonly IList<Table> _tables;

        public RestaurantService(IBus bus)
        {
            _tables = new List<Table>(10);

            for (int i = 1; i <= 10; i++)
            {
                Table table = new Table(i);
                _tables.Add(table);
            }

            _bus = bus;
        }

        public async Task<Table?> BookFreeTableAsync(int countOfPersons, Order order)
        {
            Console.WriteLine($"[ OrderId: {order.OrderId} ] Добрый день! Спасибо за обращение, я подберу столик и подтвержу вашу бронь. Вам придет уведомление.");

            Table? table = null;

            lock (_tables)
            {
                table = _tables.FirstOrDefault(t => t.SeatsCount >= countOfPersons && t.State == State.Free);
                table?.SetState(State.Booked);
                table?.SetOrder(order);
            }

            return table;
        }

        public async Task<bool> BookTableCancelAsync(Guid orderId)
        {
            Table? table = null;

            lock (_tables)
            {            
                table = _tables.FirstOrDefault(t => t?.Order?.OrderId == orderId);
                table?.SetState(State.Free);
                table?.ClearOrder();
                Console.WriteLine($" [x] Отмена бронирования столика {table?.Id}");
            }

            return true;
        }

        public Dish GetRandomDish()
        {
            Random random = new Random();

            switch(random.Next(1, 5))
            {
                case 1: return Dish.CesarSalad;
                case 2: return Dish.PizzaMargarita;
                case 3: return Dish.RibyeSteak;
                case 4: return Dish.Lasagna;
                default: return Dish.Empty;
            }
        }

    }
}
