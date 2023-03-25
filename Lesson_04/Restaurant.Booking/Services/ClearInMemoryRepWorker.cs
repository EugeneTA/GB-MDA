using System.Text;
using Microsoft.Extensions.Hosting;
using Restaurant.IdempotentLibrary.Models;
using Restaurant.IdempotentLibrary.Repositories;

namespace Restaurant.Booking.Services
{
    public class ClearInMemoryRepWorker : BackgroundService, IDisposable
    {
        private readonly IInMemoryRepository<BookingRequestModel> _repository;

        public ClearInMemoryRepWorker(
            IInMemoryRepository<BookingRequestModel> repository)
        {
            _repository = repository;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.OutputEncoding = Encoding.UTF8;
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine();
                Console.WriteLine("[x] Clearing in memory repository");
                Console.WriteLine();

                await Task.Delay(30000, stoppingToken);
                _repository.Initialize();
            }
        }
    }
}
