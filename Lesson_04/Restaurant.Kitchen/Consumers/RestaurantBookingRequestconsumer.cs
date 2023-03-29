using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.IdempotentLibrary.Models;
using Restaurant.IdempotentLibrary.Repositories;
using Restaurant.Kitchen.Services;
using Restaurant.Messages.Booking;
using Restaurant.Messages.Kitchen;


namespace Restaurant.Kitchen.Consumers
{
    public class RestaurantBookingRequestConsumer : IConsumer<IBookingRequest>
    {
        private readonly Manager _manager;
        private readonly IInMemoryRepository<BookingRequestModel> _repository;
        private readonly ILogger<RestaurantBookingRequestConsumer> _logger;

        public RestaurantBookingRequestConsumer(
            Manager manager,
            IInMemoryRepository<BookingRequestModel> repository,
            ILogger<RestaurantBookingRequestConsumer> logger)
        {
            _manager = manager;
            _repository = repository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IBookingRequest> context)
        {
            _logger.Log(LogLevel.Information, $"[ OrderId: {context.Message.OrderId} ] Consume booking request");

            var model = _repository.Get().FirstOrDefault(i => i.OrderId == context.Message.OrderId);
            var t = model?.CheckMessage(context.MessageId.ToString());

            if (model != null && model.CheckMessage(context.MessageId.ToString()))
            {
                _logger.Log(LogLevel.Warning, $"[ OrderId: {context.Message.OrderId} ] [ MessageID {context.MessageId} ] Second request");
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

            _logger.Log(LogLevel.Information, $"[ OrderId: {context.Message.OrderId} ] [ MessageID {context.MessageId} ] First request");

            var resultModel = model?.Update(requestModel, context.MessageId.ToString()) ?? requestModel;

            _repository.AddOrUpdate(resultModel);

            var rnd = new Random().Next(1000, 10000);

            //Console.WriteLine($"[OrderId {context.Message.OrderId} ] [ {context.Message.Created} ] Проверка на кухне займет: {rnd}");
            //await Task.Delay(rnd);

            var result = _manager.CheckKitchenReady(context.Message.OrderId, context.Message.PreOrder);

            if (result == false)
            {
                _logger.Log(LogLevel.Warning, $"[ OrderId: {context.Message.OrderId} ] [ MessageID {context.MessageId} ] Lazagnia in stop list");
                throw new Exception("Lazagnia in stop list"); 
            }

            await context.Publish<IKitchenReady>(new KitchenReady(context.Message.OrderId, result));

            _logger.Log(LogLevel.Information, $"[ OrderId: {context.Message.OrderId} ] [ MessageID {context.MessageId} ] Предварительный заказ на: {context.Message.PreOrder}");
        }
    }
}
