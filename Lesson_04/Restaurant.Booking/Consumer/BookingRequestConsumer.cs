using MassTransit;
using Restaurant.Booking.Models;
using Restaurant.Booking.Services;
using Restaurant.IdempotentLibrary.Models;
using Restaurant.IdempotentLibrary.Repositories;
using Restaurant.Messages.Booking;


namespace Restaurant.Booking.Consumer
{
    public class BookingRequestConsumer: IConsumer<IBookingRequest>
    {
        private readonly RestaurantService _restaurant;
        private readonly IInMemoryRepository<BookingRequestModel> _repository;

        public BookingRequestConsumer(
            RestaurantService restaurant,
            IInMemoryRepository<BookingRequestModel> repository)
        {
            _restaurant = restaurant;
            _repository = repository;

        }

        public async Task Consume(ConsumeContext<IBookingRequest> context)
        {
            Console.WriteLine($"[ OrderId: {context.Message.OrderId} ] Consume booking request");

            var model = _repository.Get().FirstOrDefault(i => i.OrderId == context.Message.OrderId);
            var t = model?.CheckMessage(context.MessageId.ToString());

            if (model != null && model.CheckMessage(context.MessageId.ToString()))
            {
                Console.WriteLine($"[ OrderId: {context.Message.OrderId} ] [ MessageID {context.MessageId} ] Second request");
                return;
            }

            var requestModel = new BookingRequestModel
                (
                    context.Message.OrderId,
                    context.Message.ClientId,
                    context.Message.PreOrder,
                    context.Message.Created,
                    context.MessageId.ToString()
                );

            Console.WriteLine($"[ OrderId: {context.Message.OrderId} ] [ MessageID {context.MessageId} ] First request");

            var resultModel = model?.Update(requestModel, context.MessageId.ToString()) ?? requestModel;
            
            _repository.AddOrUpdate(resultModel);


            var result = _restaurant.BookFreeTableAsync(1, new Order(context.Message.OrderId, context.Message.ClientId, context.Message.PreOrder));

            if (result.Result == null)
            {
                await context.Publish<IBookingCancellation>(new BookingCancellation(context.Message.OrderId, context.Message.ClientId));
            }

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
