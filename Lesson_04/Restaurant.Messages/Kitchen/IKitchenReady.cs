namespace Restaurant.Messages.Kitchen
{
    public interface IKitchenReady
    {
        public Guid OrderId { get; }
        public bool Ready { get; }
    }
}
