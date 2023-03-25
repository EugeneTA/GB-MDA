using Restaurant.Messages.Kitchen;

namespace Restaurant.Messages.Booking
{
    public class TableBooked : ITableBooked
    {
        public Guid OrderId { get; set; }

        public Guid ClientId { get; set; }

        public Dish? PreOrder { get; set; }

        public bool Success { get; set; }

        public TableBooked(Guid orderId, Guid clientId, bool success, Dish? preOrder = null)
        {
            OrderId = orderId;
            ClientId = clientId;
            PreOrder = preOrder;
            Success = success;
        }

    }
}
