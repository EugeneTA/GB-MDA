using MassTransit;
using Restaurant.Booking.Models;
using Restaurant.Booking.Services;
using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
