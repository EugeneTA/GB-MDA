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
    public class SagaTestKitchenFail
    {
        [Test]
        public async Task Saga_Booking_Request_And_Kitchen_Failed()
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
                    Dish.Lasagna,
                    DateTime.Now,
                    TimeSpan.FromSeconds(20)
                    ));

            Assert.That(await harness.Published.Any<IBookingRequest>(), Is.True);
            Assert.That(await harness.Consumed.Any<IBookingRequest>(), Is.True);

            Assert.That(await sagaHarness.Consumed.Any<IBookingRequest>(), Is.True);
            Assert.That(await sagaHarness.Created.Any(saga => saga.CorrelationId == orderId), Is.True);

            var saga = sagaHarness.Created.Contains(orderId);

            Assert.That(saga, Is.Not.Null);
            Assert.That(saga.ClientId, Is.EqualTo(clientId));

            Assert.That(await harness.Published.Any<ITableBooked>(), Is.True);
            Assert.That(await sagaHarness.Consumed.Any<ITableBooked>(), Is.True);

            Assert.That(await harness.Published.Any<IKitchenReady>(), Is.False);
            Assert.That(await sagaHarness.Consumed.Any<IKitchenReady>(), Is.False);

            Assert.That(await harness.Published.Any<INotify>(x => x.Context.Message.Message.Equals("Стол успешно забронирован")), Is.False);
            Assert.That(await harness.Published.Any<IWaitingForClient>(), Is.False);

            Assert.That(await harness.Published.Any<Fault<IBookingRequest>>(), Is.True);
        }
    }
}
