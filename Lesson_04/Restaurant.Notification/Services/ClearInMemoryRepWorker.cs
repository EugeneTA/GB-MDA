using Microsoft.Extensions.Hosting;
using Restaurant.IdempotentLibrary.Models;
using Restaurant.IdempotentLibrary.Repositories;
using System.Text;

namespace Restaurant.Notification.Consumers
{
    public class ClearInMemoryRepWorker : BackgroundService, IDisposable
    {
        private readonly IInMemoryRepository<NotifyModel> _repository;

        public ClearInMemoryRepWorker(
            IInMemoryRepository<NotifyModel> repository)
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
