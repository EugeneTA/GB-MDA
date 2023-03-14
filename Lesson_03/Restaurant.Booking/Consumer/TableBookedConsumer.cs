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
    public class TableBookedConsumer: IConsumer<ITableBooked>
    {
        private readonly RestaurantService _restaurant;

        public TableBookedConsumer(RestaurantService restaurant)
        {
            _restaurant = restaurant;
        }

        public Task Consume(ConsumeContext<ITableBooked> context)
        {
            var result = context.Message.Success;

            if (result == false) 
            {
                _restaurant.BookTableCancelAsync(context.Message.OrderId);
            }

            return context.ConsumeCompleted;
        }
    }
}
