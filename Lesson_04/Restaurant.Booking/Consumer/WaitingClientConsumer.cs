using MassTransit;
using Restaurant.Messages.Client;

namespace Restaurant.Booking.Consumer
{
    public class WaitingClientConsumer: IConsumer<IWaitingForClient>
    {

        public WaitingClientConsumer()
        {
        }

        public async Task Consume(ConsumeContext<IWaitingForClient> context)
        {
            Console.WriteLine($"[ OrderId: {context.Message.OrderId} ] Ожидаем клиента.");
            
            await Task.Delay(new Random().Next(7000,15000));

            await context.Publish<IClientArrived>(
                new ClientArrived(
                context.Message.OrderId,
                context.Message.ClientId
                ));
        }
    }
}
