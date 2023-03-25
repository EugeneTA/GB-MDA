namespace Restaurant.IdempotentLibrary.Models
{
    public class NotifyModel
    {
        public Guid OrderId { get; private set; }
        public Guid ClientId { get; private set; }
        public string Message { get; private set; }

        private readonly IList<string> _messageIds = new List<string>();

        public NotifyModel(Guid orderId, Guid clientId, string message, string messageIds)
        {
            OrderId = orderId;
            ClientId = clientId;
            Message = message;
            _messageIds.Add(messageIds);
        }

        public NotifyModel Update(NotifyModel model, string message)
        {
            if (String.IsNullOrEmpty(message) == false) _messageIds.Add(message);

            if (model != null)
            {
                OrderId = model.OrderId;
                ClientId = model.ClientId;
                Message = model.Message;
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
