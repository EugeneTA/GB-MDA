using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Messages
{
    public class BookingRequest: IBookingRequest
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }
        public Dish PreOrder { get; }
        public DateTime Created { get; }

        public BookingRequest(Guid orderId, Guid clientId, Dish preOrder, DateTime created)
        {
            OrderId = orderId;
            ClientId = clientId;
            PreOrder = preOrder;
            Created = created;
        }

    }
}
