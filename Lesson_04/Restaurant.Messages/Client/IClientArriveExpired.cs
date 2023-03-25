namespace Restaurant.Messages.Client
{
    public interface IClientArriveExpired
    {
        public Guid OrderId { get; }
        public Guid ClientId { get; }
    }
}
