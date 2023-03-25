namespace Restaurant.Messages.Booking
{
    public class BookingCancellation : IBookingCancellation
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }

        public BookingCancellation(Guid orderId, Guid clientId)
        {
            OrderId = orderId;
            ClientId = clientId;
        }
    }
}
