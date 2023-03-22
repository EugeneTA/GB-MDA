using MassTransit;
using Restaurant.Kitchen.Services;
using Restaurant.Messages;


namespace Restaurant.Kitchen.Consumers
{
    public class RestaurantBookingRequestConsumer : IConsumer<IBookingRequest>
    {
        private readonly Manager _manager;

        public RestaurantBookingRequestConsumer(Manager manager)
        {
            _manager = manager;
        }

        public async Task Consume(ConsumeContext<IBookingRequest> context)
        {
            var rnd = new Random().Next(1000, 10000);

            //Console.WriteLine($"[OrderId {context.Message.OrderId} ] [ {context.Message.Created} ] Проверка на кухне займет: {rnd}");
            //await Task.Delay(rnd);

            var result = _manager.CheckKitchenReady(context.Message.OrderId, context.Message.PreOrder);

            if (result == false) { throw new Exception("Lazagnia in stop list"); }

            await context.Publish<IKitchenReady>(new KitchenReady(context.Message.OrderId, result));

            Console.WriteLine($"[OrderId {context.Message.OrderId} ] [ {context.Message.Created} ] Предварительный заказ на: {context.Message.PreOrder}");
        }
    }
}
