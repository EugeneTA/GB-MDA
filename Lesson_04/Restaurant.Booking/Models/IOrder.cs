using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Booking.Models
{
    public interface IOrder
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }
    }
}
