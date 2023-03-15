using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Messages
{
    public interface IBookingRequest
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }
        public Dish PreOrder { get; }
        public DateTime Created { get; }
        public TimeSpan  ArrivingTime { get; }
    }
}
