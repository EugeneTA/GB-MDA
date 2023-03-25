namespace Restaurant.Messages.Client
{
    public class ClientArriveExpired : IClientArriveExpired
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }

        public ClientArriveExpired(Guid orderId, Guid clientId)
        {
            OrderId = orderId;
            ClientId = clientId;
        }
    }
}
