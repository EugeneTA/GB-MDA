using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Messages
{
    public class TableBooked : ITableBooked
    {
        public Guid OrderId { get; set; }

        public Guid ClientId { get; set; }

        public Dish? PreOrder { get; set; }

        public bool Success { get; set; }

        public TableBooked(Guid orderId, Guid clientId, bool success, Dish? preOrder = null)
        {
            OrderId = orderId;
            ClientId = clientId;
            PreOrder = preOrder;
            Success = success;
        }

    }
}
