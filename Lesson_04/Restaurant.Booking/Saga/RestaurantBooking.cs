using MassTransit;

namespace Restaurant.Booking.Saga
{
    public class RestaurantBooking : SagaStateMachineInstance
    {
        // идентификатор для соотнесения всех сообщений друг с другом
        public Guid CorrelationId { get; set; }
        // текущее состояние саги (по умолчанию присутствуют Initial=1 и Final=2)
        public int CurrentState { get; set; }
        // Идентификатор заказа
        public Guid OrderId { get; set; }
        // Идентификатор клиента
        public Guid ClientId { get; set; }
        // маркировка для "композиции" событий (наш случай с кухней и забронированным столом)
        public int ReadyEventStatus { get; set; }
        // время ожидания до прихода клиента
        public TimeSpan ArriveTimeout { get; set; }
        // пометка о том, что заявка просрочена
        public Guid? ExpirationId { get; set; }
        // пометка о том, что клиент не пришел
        public Guid? ArriveExpirationId { get; set; }
    }
}
