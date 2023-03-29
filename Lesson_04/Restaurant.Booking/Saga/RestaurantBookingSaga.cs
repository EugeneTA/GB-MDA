using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messages.Booking;
using Restaurant.Messages.Client;
using Restaurant.Messages.Kitchen;
using Restaurant.Messages.Notification;

namespace Restaurant.Booking.Saga
{
    public sealed class RestaurantBookingSaga: MassTransitStateMachine<RestaurantBooking>
    {
        private readonly ILogger<RestaurantBookingSaga> _logger;

        public State AwaitingBookingApproved { get; private set; }
        public State AwaitingClientArrived { get; private set; }
        public Event<IBookingRequest> BookingRequested { get; private set; }
        public Event<IBookingCancellation> BookingCancelled { get; private set; }
        public Event<ITableBooked> TableBooked { get; private set; }
        public Event<IKitchenReady> KitchenReady { get; private set; }
        public Event<IClientArrived> ClientArrived { get; private set; }
        public Event<Fault<IBookingRequest>> BookingRequestFault { get; private set; }
        public Schedule<RestaurantBooking,IBookingExpire> BookingExpired { get; private set; }
        public Schedule<RestaurantBooking, IClientArriveExpired> ClientArriveExpired { get; private set; }
        public Event BookingApproved { get; private set; }

        public RestaurantBookingSaga(ILogger<RestaurantBookingSaga> logger)
        {
            _logger = logger;

            InstanceState(x => x.CurrentState);

            Event(() => BookingRequested,
                x => x.CorrelateById(context => context.Message.OrderId).SelectId(context => context.Message.OrderId));

            Event(() => BookingCancelled,
                x => x.CorrelateById(context => context.Message.OrderId));

            Event(() => TableBooked,
                x => x.CorrelateById(context => context.Message.OrderId));

            Event(() => KitchenReady,
                x => x.CorrelateById(context => context.Message.OrderId));

            Event(() => ClientArrived,
                x => x.CorrelateById(context => context.Message.OrderId));

            Event(() => BookingRequestFault,
                x => x.CorrelateById(context => context.Message.FaultId));

            CompositeEvent(() => BookingApproved,
                x => x.ReadyEventStatus, KitchenReady, TableBooked);

            Schedule(() => BookingExpired,
                x => x.ExpirationId, x =>
                {
                    x.Delay = TimeSpan.FromSeconds(15);
                    x.Received = e => e.CorrelateById(context => context.Message.OrderId);
                });

            Schedule(() => ClientArriveExpired,
                    x => x.ArriveExpirationId, x =>
                    {
                        x.Delay = TimeSpan.FromSeconds(15);
                        x.Received = e => e.CorrelateById(context => context.Message.OrderId);
                    });

            Initially(
                When(BookingRequested)
                .Then(context =>
                {
                    context.Instance.CorrelationId = context.Data.OrderId;
                    context.Instance.OrderId = context.Data.OrderId;
                    context.Instance.ClientId = context.Data.ClientId;
                    context.Instance.ArriveTimeout = context.Data.ArrivingTime;
                    _logger.Log(LogLevel.Information, $"[ OrderId: {context.Instance.OrderId} ] Starting Saga: {context.Data.Created}");
                })
                .Schedule(BookingExpired, context => new BookingExpire(context.Instance.OrderId))
                .TransitionTo(AwaitingBookingApproved)
            );

            During(AwaitingBookingApproved,

                // В случае возникновения ошибки
                When(BookingRequestFault)
                .Unschedule(BookingExpired)
                .Then(ctx => _logger.Log(LogLevel.Error, $"[ OrderId: {ctx.Instance.OrderId} ] Произошла ошибка!"))
                .Publish(ctx => (INotify)new Notify(ctx.Instance.OrderId, ctx.Instance.ClientId, $"Приносим свои извинения, забронировать стол не получилось."))
                //.Publish(ctx => (IBookingCancellation)new BookingCancellation(ctx.Instance.OrderId))
                .Finalize(),

                // Все столы заняты
                When(BookingCancelled)
                .Unschedule(BookingExpired)
                .Publish(context => (INotify)new Notify(context.Instance.OrderId, context.Instance.ClientId, "Извините, но все столы заняты"))
                .Finalize(),

                // Подтверждение бронирования
                When(BookingApproved)
                .Unschedule(BookingExpired)
                .Publish(context => (INotify)new Notify(context.Instance.OrderId, context.Instance.ClientId, "Стол успешно забронирован"))
                .Schedule(ClientArriveExpired,
                    context => context.Init<IClientArriveExpired>(new ClientArriveExpired(context.Instance.CorrelationId, context.Instance.ClientId)),
                    context => context.Instance.ArriveTimeout)
                .Publish(context =>
                (IWaitingForClient)new WaitingForClient(context.Instance.OrderId, context.Instance.ClientId))
                .TransitionTo(AwaitingClientArrived),


                // Отмена заказа по таймауту (не пришли ответы от кухни или наличия свободных столов)
                When(BookingExpired.Received)
                .Then(context => _logger.Log(LogLevel.Warning, $"[ OrderId: {context.Instance.OrderId} ] Отмена заказа {context.Instance.OrderId}"))
                .Publish(context =>
                (INotify)new Notify(context.Instance.OrderId, context.Instance.ClientId, $"Извините, ваш заказ отменен.")).Finalize()
            );

            // Ожидаем прихода гостя
            During(AwaitingClientArrived,
                
                // Гость пришел
                When(ClientArrived)
                .Unschedule(ClientArriveExpired)
                .Then(context => _logger.Log(LogLevel.Information, $"[ OrderId: {context.Instance.OrderId} ] Гость {context.Instance.ClientId} прибыл."))
                .Finalize(),

                // Гость опоздал
                When(ClientArriveExpired.Received)
                .Then(context => _logger.Log(LogLevel.Information, $"[ OrderId: {context.Instance.OrderId} ] Отмена заказа. Гость {context.Instance.ClientId} не пришел."))
                .Publish(context =>
                (INotify)new Notify(context.Instance.OrderId, context.Instance.ClientId, $"Извините, но Вы не пришли в указанное время. Ваш заказ отменен.")).Finalize()
               ); 

            SetCompletedWhenFinalized();

        }
    }
}
