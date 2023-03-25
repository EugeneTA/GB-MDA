namespace Restaurant.Messages.Client
{
    public interface IClientArrived
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }
    }
}
