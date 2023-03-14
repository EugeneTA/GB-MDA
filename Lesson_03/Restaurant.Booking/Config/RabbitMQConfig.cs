using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Booking.Config
{
    public class RabbitMQConfig
    {
        public string? QueueName { get; set; }
        public string? HostName { get; set; }
        public string? VirtualHostName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
