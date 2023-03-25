namespace Restaurant.Messages.Notification
{
    public class Notify : INotify
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }
        public string Message { get; }

        public Notify(Guid orderId, Guid clientId, string message)
        {
            OrderId = orderId;
            ClientId = clientId;
            Message = message;
        }

    }
}
