using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.IdempotentLibrary.Models;
using Restaurant.IdempotentLibrary.Repositories;
using Restaurant.Messages.Notification;
using Restaurant.Notification;
using Restaurant.Notification.Consumers;

namespace Restaurant.Tests
{
    public class NotificationTests
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
                    cfg.AddConsumer<NotifyConsumer>();
                })
                .AddLogging()
                .AddTransient<Notifier>()
                .AddSingleton<IInMemoryRepository<NotifyModel>, InMemoryRepository<NotifyModel>>()
                .BuildServiceProvider(true);

            _harness = _provider.GetTestHarness();

            OrderId = Guid.NewGuid();
            ClientId = Guid.NewGuid();

            await _harness.Start();

            await _harness.Bus.Publish(
                (INotify)new Notify(
                    OrderId,
                    ClientId,
                    "Test message"
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
            Assert.That(await _harness.Consumed.Any<INotify>());
        }

        [Test]
        public async Task Booking_Request_Consumer_Published_Kitchen_Ready_Message()
        {
            var consumer = _harness.GetConsumerHarness<NotifyConsumer>();

            Assert.That(consumer.Consumed.Select<INotify>().Any(x => 
                x.Context.Message.OrderId == OrderId && x.Context.Message.Message.Equals("Test message") 
                ),
                Is.True);
        }

    }
}
