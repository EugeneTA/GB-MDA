namespace Restaurant.Messages.Client
{
    public interface IWaitingForClient
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }
    }
}
