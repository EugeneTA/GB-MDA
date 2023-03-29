using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Restaurant.IdempotentLibrary.Models;
using Restaurant.IdempotentLibrary.Repositories;

namespace Restaurant.Booking.Services
{
    public class ClearInMemoryRepWorker : BackgroundService, IDisposable
    {
        private readonly IInMemoryRepository<BookingRequestModel> _repository;
        private readonly ILogger<ClearInMemoryRepWorker> _logger;

        public ClearInMemoryRepWorker(
            IInMemoryRepository<BookingRequestModel> repository,
            ILogger<ClearInMemoryRepWorker> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.OutputEncoding = Encoding.UTF8;
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.Log(LogLevel.Information, "[x] Clearing in memory repository");

                await Task.Delay(30000, stoppingToken);
                _repository.Initialize();
            }
        }
    }
}
