namespace Restaurant.Messages.Booking
{
    public interface IBookingCancellation
    {
        public Guid OrderId { get; }
    }
}
