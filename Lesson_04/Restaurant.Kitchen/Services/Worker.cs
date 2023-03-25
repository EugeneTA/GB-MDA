using MassTransit;
using Microsoft.Extensions.Hosting;
using Restaurant.Messages.Kitchen;
using System.Text;

namespace Restaurant.Kitchen.Services
{
    public class Worker : BackgroundService, IDisposable
    {
        private readonly IBus _bus;
        private readonly Manager _manager;


        public Worker(
            IBus bus,
            Manager manager)
        {
            _bus = bus;
            _manager = manager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.OutputEncoding = Encoding.UTF8;
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(15000, stoppingToken);
                Dish dish = GetRandomDish();

                Console.WriteLine($"Блюдо {dish} в стоп-листе.");

                //_manager.DishInStopList(dish);
            }
        }

        private Dish GetRandomDish()
        {
            Random random = new Random();

            switch (random.Next(1, 4))
            {
                case 1: return Dish.CesarSalad;
                case 2: return Dish.PizzaMargarita;
                case 3: return Dish.RibyeSteak;
                default: return Dish.Empty;
            }
        }
    }
}
