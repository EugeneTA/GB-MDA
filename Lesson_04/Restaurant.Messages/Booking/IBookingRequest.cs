using Restaurant.Messages.Kitchen;

namespace Restaurant.Messages.Booking
{
    public interface IBookingRequest
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }
        public Dish PreOrder { get; }
        public DateTime Created { get; }
        public TimeSpan ArrivingTime { get; }
    }
}
