using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Booking.Models
{
    public class Order : IOrder
    {
        public Guid OrderId { get; }

        public Guid ClientId { get; }

        public Dish Dish { get; }

        public Order(Guid orderId, Guid clientId, Dish dish)
        {
            OrderId = orderId;
            ClientId = clientId;
            Dish = dish;
        }
    }
}
