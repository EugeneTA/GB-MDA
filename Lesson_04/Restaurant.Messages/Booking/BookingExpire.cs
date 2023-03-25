namespace Restaurant.Messages.Booking
{
    public class BookingExpire : IBookingExpire
    {
        public Guid OrderId { get; }

        public BookingExpire(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
