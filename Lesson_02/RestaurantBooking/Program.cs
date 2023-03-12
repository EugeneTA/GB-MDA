using Messaging;
using RabbitMQ.Client;
using System.Diagnostics;

namespace RestaurantBooking
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ConnectionSettings connectionSettings = new ConnectionSettings();  

            Restaurant restaurant = new Restaurant(new Producer(
                connectionSettings.QueueName,
                connectionSettings.HostName,
                connectionSettings.VirtualHostName,
                connectionSettings.UserName,
                connectionSettings.Password
                ));

            TimerStateObject timerStateObject = new TimerStateObject()
            {
                ExchangeNameParam = "notificatons",
                ExchangeTypeParam = ExchangeType.Fanout
            };

            Timer timer = new Timer(
                restaurant.CancelAllBookings,
                timerStateObject, 
                1000 * 20,
                1000 * 60);

            while (true)
            {
                await Task.Delay(10000);

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                restaurant.BookFreeTableAsync("notificatons", ExchangeType.Fanout, 1);

                Console.WriteLine("Спасибо за ваше обращение!");
                stopWatch.Stop();
                var timeElapsed = stopWatch.Elapsed;

                Console.WriteLine($"{timeElapsed.Seconds:00}:{timeElapsed.Milliseconds:00}");

            }
        }
    }
}