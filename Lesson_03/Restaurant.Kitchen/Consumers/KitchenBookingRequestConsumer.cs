using MassTransit;
using Restaurant.Kitchen.Services;
using Restaurant.Messages;


namespace Restaurant.Kitchen.Consumers
{
    public class KitchenBookingRequestConsumer : IConsumer<IBookingRequest>
    {
        private readonly Manager _manager;

        public KitchenBookingRequestConsumer(Manager manager)
        {
            _manager = manager;
        }

        public async Task Consume(ConsumeContext<IBookingRequest> context)
        {
            Console.WriteLine($"[OrderId {context.Message.OrderId}][{context.Message.Created}]");

            var result = _manager.CheckKitchenReady(context.Message.OrderId, context.Message.PreOrder);

            await context.Publish<IKitchenReady>(new KitchenReady(context.Message.OrderId, result));
        }
    }
}
