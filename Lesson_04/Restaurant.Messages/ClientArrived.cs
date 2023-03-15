
namespace Restaurant.Messages
{
    public class ClientArrived: IClientArrived
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }

        public ClientArrived(Guid orderId, Guid clientId)
        {
            ClientId = clientId;
            OrderId = orderId;
        }
    }
}
