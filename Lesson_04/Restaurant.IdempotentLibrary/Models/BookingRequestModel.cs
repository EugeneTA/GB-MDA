using Restaurant.Messages.Kitchen;

namespace Restaurant.IdempotentLibrary.Models
{
    public class BookingRequestModel
    {
        public Guid OrderId { get; private set; }
        public Guid ClientId { get; private set; }
        public Dish PreOrder { get; private set; }
        public DateTime Created { get; private set; }

        private readonly IList<string> _messageIds = new List<string>();

        public BookingRequestModel(Guid orderId, Guid clientId, Dish preOrder, DateTime created, string messageIds)
        {
            OrderId = orderId;
            ClientId = clientId;
            PreOrder = preOrder;
            Created = created;
            _messageIds.Add(messageIds);
        }

        public BookingRequestModel Update (BookingRequestModel model, string message)
        {
            if (String.IsNullOrEmpty(message) == false) _messageIds.Add(message);

            if (model != null)
            {
                OrderId = model.OrderId;
                ClientId = model.ClientId;
                PreOrder = model.PreOrder;
                Created = model.Created;
            }

            return this;
        }

        public bool CheckMessage(string message)
        {
            if (string.IsNullOrEmpty(message) == true) return false;
            return _messageIds.Contains(message);
        }
    }
}
