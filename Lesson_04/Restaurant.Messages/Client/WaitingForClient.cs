namespace Restaurant.Messages.Client
{
    public class WaitingForClient : IWaitingForClient
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }

        public WaitingForClient(Guid orderId, Guid clientId)
        {
            OrderId = orderId;
            ClientId = clientId;
        }
    }
}
