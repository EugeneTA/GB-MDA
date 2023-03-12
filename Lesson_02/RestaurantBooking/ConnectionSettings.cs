using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantBooking
{
    public class ConnectionSettings
    {
        public string QueueName  => "notificatons";
        public string HostName => "cow.rmq2.cloudamqp.com";
        public string VirtualHostName => "srgicxjt";
        public string UserName => "srgicxjt";
        public string Password => "ztUKEjNXQxDlxha5npbLMSKc-Ecrf_gx";
    }
}
