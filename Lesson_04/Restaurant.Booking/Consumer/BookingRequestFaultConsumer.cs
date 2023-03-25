using MassTransit;
using Restaurant.Messages.Booking;

namespace Restaurant.Booking.Consumer
{
    public class BookingRequestFaultConsumer : IConsumer<Fault<IBookingRequest>>
    {
        public Task Consume(ConsumeContext<Fault<IBookingRequest>> context)
        {
            Console.WriteLine($"[ OrderId: {context.Message.Message.OrderId} ]. Отмена в зале.");
            return context.ConsumeCompleted;
        }
    }
}
