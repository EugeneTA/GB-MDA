using MassTransit;
using Restaurant.Messages.Kitchen;

namespace Restaurant.Kitchen.Services
{
    public class Manager
    {
        public readonly IBus _bus;

        public Manager(IBus bus)
        {
            _bus = bus;
        }

        public bool CheckKitchenReady(Guid orderId, Dish? dish)
        {
            // Кухня не принимает заказы на лазанью
            return dish == Dish.Lasagna ? false : true;
        }

    }
}
