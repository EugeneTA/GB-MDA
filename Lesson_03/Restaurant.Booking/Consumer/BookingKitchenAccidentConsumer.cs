using MassTransit;
using Restaurant.Booking.Services;
using Restaurant.Messages;


namespace Restaurant.Notification.Consumers
{
    public class BookingKitchenAccidentConsumer : IConsumer<IKitchenAccident>
    {
        private readonly RestaurantService _restaurant;

        public BookingKitchenAccidentConsumer(RestaurantService restaurant)
        {
            _restaurant = restaurant;
        }


        public Task Consume(ConsumeContext<IKitchenAccident> context)
        {
            _restaurant.BookTableCancelAsync(context.Message.OrderId);

            return context.ConsumeCompleted;
        }
    }
}
