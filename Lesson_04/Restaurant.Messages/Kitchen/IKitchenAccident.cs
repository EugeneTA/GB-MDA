namespace Restaurant.Messages.Kitchen
{
    public interface IKitchenAccident
    {
        public Guid OrderId { get; }
        public Dish? Dish { get; }
    }
}
