using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messages.Client;

namespace Restaurant.Booking.Consumer
{
    public class WaitingClientConsumer: IConsumer<IWaitingForClient>
    {
        private readonly ILogger<WaitingClientConsumer> _logger;

        public WaitingClientConsumer(ILogger<WaitingClientConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IWaitingForClient> context)
        {
            _logger.Log(LogLevel.Information, $"[ OrderId: {context.Message.OrderId} ] Ожидаем клиента.");
            
            await Task.Delay(new Random().Next(7000,15000));

            await context.Publish<IClientArrived>(
                new ClientArrived(
                context.Message.OrderId,
                context.Message.ClientId
                ));
        }
    }
}
