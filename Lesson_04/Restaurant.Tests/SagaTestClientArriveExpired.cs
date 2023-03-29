using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Booking.Consumer;
using Restaurant.Booking.Saga;
using Restaurant.Booking.Services;
using Restaurant.IdempotentLibrary.Models;
using Restaurant.IdempotentLibrary.Repositories;
using Restaurant.Kitchen.Consumers;
using Restaurant.Kitchen.Services;
using Restaurant.Messages.Booking;
using Restaurant.Messages.Client;
using Restaurant.Messages.Kitchen;
using Restaurant.Messages.Notification;
using Restaurant.Notification;
using Restaurant.Notification.Consumers;

namespace Restaurant.Tests
{
    public class SagaTestClientArriveExpired
    {
        [Test]
        public async Task Saga_Booking_Request_Confirmed_And_Client_Arrive_Expired()
        {
            using var provider = new ServiceCollection()
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<BookingRequestConsumer>();
                    cfg.AddConsumer<WaitingClientConsumer>();
                    cfg.AddConsumer<RestaurantBookingRequestConsumer>();
                    cfg.AddConsumer<NotifyConsumer>();

                    cfg.AddSagaStateMachine<RestaurantBookingSaga, RestaurantBooking>().InMemoryRepository();
                    cfg.AddSagaStateMachineTestHarness<RestaurantBookingSaga, RestaurantBooking>();
                    cfg.AddDelayedMessageScheduler();
                })
                .AddLogging()
                .AddTransient<RestaurantService>()
                .AddTransient<Manager>()
                .AddTransient<Notifier>()
                .AddSingleton<IInMemoryRepository<BookingRequestModel>, InMemoryRepository<BookingRequestModel>>()
                .AddSingleton<IInMemoryRepository<NotifyModel>, InMemoryRepository<NotifyModel>>()
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<InMemoryTestHarness>();
            harness.OnConfigureInMemoryBus += cfg => cfg.UseDelayedMessageScheduler();

            await harness.Start();

            var orderId = Guid.NewGuid();
            var clientId = Guid.NewGuid();

            var sagaHarness = provider.GetRequiredService<ISagaStateMachineTestHarness<RestaurantBookingSaga, RestaurantBooking>>();

            await harness.Bus.Publish(
                (IBookingRequest)new BookingRequest(
                    orderId,
                    clientId,
                    Dish.Empty,
                    DateTime.Now,
                    TimeSpan.FromSeconds(5)
                    ));

            Assert.That(await harness.Published.Any<IBookingRequest>(), Is.True);
            Assert.That(await harness.Consumed.Any<IBookingRequest>(), Is.True);

            Assert.That(await sagaHarness.Consumed.Any<IBookingRequest>(), Is.True);
            Assert.That(await sagaHarness.Created.Any(saga => saga.CorrelationId == orderId), Is.True);

            var saga = sagaHarness.Created.Contains(orderId);
            //saga.ArriveTimeout = TimeSpan.FromSeconds(5);

            Assert.That(saga, Is.Not.Null);
            Assert.That(saga.ClientId, Is.EqualTo(clientId));

            Assert.That(await harness.Published.Any<ITableBooked>(), Is.True);
            Assert.That(await sagaHarness.Consumed.Any<ITableBooked>(), Is.True);

            Assert.That(await harness.Published.Any<IKitchenReady>(), Is.True);
            Assert.That(await sagaHarness.Consumed.Any<IKitchenReady>(), Is.True);

            Assert.That(await harness.Published.Any<INotify>(x => x.Context.Message.Message.Equals("Стол успешно забронирован")), Is.True);
            Assert.That(await harness.Published.Any<IWaitingForClient>(), Is.True);

            Assert.That(await sagaHarness.Consumed.Any<IClientArriveExpired>(), Is.True);

            Assert.That(await harness.Published.Any<INotify>(x => x.Context.Message.Message == "Извините, но Вы не пришли в указанное время. Ваш заказ отменен."), Is.True);
        }
    }
}
