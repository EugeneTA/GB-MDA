using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Messages
{
    public class BookingCancellation: IBookingCancellation
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }

        public BookingCancellation(Guid orderId, Guid clientId)
        {
            OrderId = orderId;
            ClientId = clientId;
        }
    }
}
