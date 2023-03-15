using MassTransit;
using Restaurant.Booking.Models;
using Restaurant.Booking.Services;
using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Booking.Consumer
{
    public class BookingRequestConsumer: IConsumer<IBookingRequest>
    {
        private readonly RestaurantService _restaurant;

        public BookingRequestConsumer(RestaurantService restaurant)
        {
            _restaurant = restaurant;
        }

        public async Task Consume(ConsumeContext<IBookingRequest> context)
        {
            Console.WriteLine($"OrderId: {context.Message.OrderId}");

            var result = _restaurant.BookFreeTableAsync(1, new Order(context.Message.OrderId, context.Message.ClientId, context.Message.PreOrder));

            await context.Publish<ITableBooked>(
                new TableBooked(
                context.Message.OrderId,
                context.Message.ClientId,
                result.Result == null ? false : true,
                context.Message.PreOrder
                ));
        }
    }
}
