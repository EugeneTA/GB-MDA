using MassTransit;
using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Notification.Consumers
{
    public class NotifierKitchenAccidentConsumer : IConsumer<IKitchenAccident>
    {
        private readonly Notifier _notifier;

        public NotifierKitchenAccidentConsumer(Notifier notifier)
        {
            _notifier = notifier;
        }


        public Task Consume(ConsumeContext<IKitchenAccident> context)
        {
            _notifier.DishAccident(context.Message.OrderId, context.Message.Dish);

            return context.ConsumeCompleted;
        }
    }
}
