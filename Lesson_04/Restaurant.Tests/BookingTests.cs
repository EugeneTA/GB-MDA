using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Booking.Consumer;
using Restaurant.Booking.Services;
using Restaurant.IdempotentLibrary.Models;
using Restaurant.IdempotentLibrary.Repositories;
using Restaurant.Kitchen.Consumers;
using Restaurant.Messages.Booking;
using Restaurant.Messages.Client;
using Restaurant.Messages.Kitchen;

namespace Restaurant.Tests
{
    public class BookingTests
    {
        private ServiceProvider? _provider;
        private ITestHarness? _harness;
        public Guid OrderId;
        public Guid ClientId;


        [OneTimeSetUp]
        public async Task Setup()
        {
            _provider = new ServiceCollection()
                .AddMassTransitTestHarness(cfg =>
                {
                    cfg.AddConsumer<BookingRequestConsumer>();
                    cfg.AddConsumer<WaitingClientConsumer>();
                    cfg.AddConsumer<RestaurantBookingRequestConsumer>();
                })
                .AddLogging()
                .AddTransient<RestaurantService>()
                .AddSingleton<IInMemoryRepository<BookingRequestModel>, InMemoryRepository<BookingRequestModel>>()
                .BuildServiceProvider(true);

            _harness = _provider.GetTestHarness();

            OrderId = Guid.NewGuid();
            ClientId = Guid.NewGuid();

            await _harness.Start();

            await _harness.Bus.Publish(
                (IBookingRequest)new BookingRequest(
                    OrderId,
                    ClientId,
                    Dish.Empty,
                    DateTime.Now,
                    TimeSpan.FromSeconds(5)
                    ));
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await _harness.OutputTimeline(TestContext.Out, opt => opt.Now().IncludeAddress());
            await _provider.DisposeAsync();
        }

        [Test]
        public async Task Any_Booking_Request_Consumed()
        {
            Assert.That(await _harness.Consumed.Any<IBookingRequest>());
        }

        [Test]
        public async Task Booking_Request_Consumer_Published_Table_Booked_Message()
        {
            var consumer = _harness.GetConsumerHarness<BookingRequestConsumer>();

            Assert.That(consumer.Consumed.Select<IBookingRequest>().Any(x => x.Context.Message.OrderId == OrderId), Is.True);
            Assert.That(_harness.Published.Select<ITableBooked>().Any(x => x.Context.Message.OrderId == OrderId), Is.True);
        }

        [Test]
        public async Task Any_Waiting_For_Client_Request_Consumed()
        {
            await _harness.Bus.Publish(
                (IWaitingForClient)new WaitingForClient(
                    OrderId,
                    ClientId
                    ));

            Assert.That(await _harness.Consumed.Any<IWaitingForClient>());
        }
    }
}