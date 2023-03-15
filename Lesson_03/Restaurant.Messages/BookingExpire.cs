using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Messages
{
    public class BookingExpire: IBookingExpire
    {
        public Guid OrderId { get; }

        public BookingExpire(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
