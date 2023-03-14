using MassTransit;
using Restaurant.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Kitchen.Services
{
    public class Manager
    {
        public readonly IBus _bus;
        public readonly ConcurrentDictionary<Guid, Dish?> _orders = new();

        public Manager(IBus bus)
        {
            _bus = bus;
        }

        public void CheckKitchenReady(Guid orderId, Dish? dish)
        {
            //Random random = new Random();

            //bool accepted = random.Next(1, 10) > 5 ? true : false;

            //if (accepted)
            //{
            //    Console.WriteLine($"Заказ {orderId} подтвержден. Кухня может выполнить заказ.");
            //}
            //else
            //{
            //    Console.WriteLine($"Заказ {orderId} отклонен. Кухня не может выполнить заказ.");
            //}


            Console.WriteLine($"Заказ {orderId} подтвержден. Кухня может выполнить заказ.");
            _orders.TryAdd(orderId, dish);
            _bus.Publish<IKitchenReady>(new KitchenReady(orderId, true));

        }

        public void DishInStopList(Dish dish)
        {

            foreach (var order in _orders)
            {
                if (order.Value == dish)
                {
                    _bus.Publish<IKitchenAccident>(new KitchenAccident(order.Key, order.Value));
                    _orders.TryRemove(order);

                }
            }
        }
    }
}
