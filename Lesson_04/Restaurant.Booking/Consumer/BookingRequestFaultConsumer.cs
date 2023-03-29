using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messages.Booking;

namespace Restaurant.Booking.Consumer
{
    public class BookingRequestFaultConsumer : IConsumer<Fault<IBookingRequest>>
    {
        private readonly ILogger<BookingRequestFaultConsumer> _logger;

        public BookingRequestFaultConsumer(ILogger<BookingRequestFaultConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<Fault<IBookingRequest>> context)
        {
            _logger.Log(LogLevel.Error, $"[ OrderId: {context.Message.Message.OrderId} ]. Отмена в зале.");
            return context.ConsumeCompleted;
        }
    }
}
