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

        public Manager(IBus bus)
        {
            _bus = bus;
        }

        public bool CheckKitchenReady(Guid orderId, Dish? dish)
        {
            return true;
        }

    }
}
