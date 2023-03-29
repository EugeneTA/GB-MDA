using Microsoft.Extensions.Logging;

namespace Restaurant.Notification
{
    public class Notifier
    {
        private readonly ILogger<Notifier> _logger;

        public Notifier(ILogger<Notifier> logger)
        {
            _logger = logger;
        }

        public void Notify(Guid orderId, Guid clientId, string message)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            _logger.Log(LogLevel.Information, $"[ OrderId: {orderId} ] Уважаемый клиент [ {clientId} ]! {message}");
        }
    }
}
