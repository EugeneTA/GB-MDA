using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Messages
{
    public class KitchenReady : IKitchenReady
    {
        public Guid OrderId { get; }

        public bool Ready { get; }

        public KitchenReady(Guid orderId, bool ready)
        {
            OrderId = orderId;
            Ready = ready;
        }
    }
}
