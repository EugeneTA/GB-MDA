namespace Restaurant.Messages
{
    public class KitchenAccident : IKitchenAccident
    {
        public Guid OrderId { get; }
        public Dish? Dish { get; }

        public KitchenAccident(Guid orderId, Dish? dish)
        {
            OrderId = orderId;
            Dish = dish;
        }

    }
}
