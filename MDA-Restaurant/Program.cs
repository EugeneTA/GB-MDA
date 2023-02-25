using MDA_Restaurant.Models;
using System.Diagnostics;
using System.Xml.Serialization;

namespace MDA_Restaurant
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Restaurant restaurant = new Restaurant();

            Timer timer = new Timer(restaurant.CancelAllBookings, new AutoResetEvent(false), 1000*20, 1000*40);

            while (true)
            {
                Console.WriteLine("Добрый день! Выберите действие:\n1 - Забронировать столик с уведомлением по смс (асинхронно)" +
                    "\n2 - Забронировать столик по телефону (синхронно)" + 
                    "\n3 - Отменить бронь столика с уведомлением по смс (асинхронно)" +
                    "\n4 - Отменить бронь столика по телефону (синхронно)");

                if (!int.TryParse(Console.ReadLine(), out int choice) || choice is not (1 or 2 or 3 or 4))
                {
                    Console.WriteLine("Введите число от 1 до 4.");
                    continue;
                }

                int table = 0;

                if (choice is (3 or 4))
                {
                    Console.WriteLine("Введите номер столика для отмены бронирования:");
                    if (!int.TryParse(Console.ReadLine(), out table))
                    {
                        Console.WriteLine("Извините, но вы не указали номер столика.");
                        continue;
                    }
                }

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                switch (choice)
                {
                    case 1: 
                        restaurant.BookFreeTableAsync(1);
                        break;
                    case 2:
                        restaurant.BookFreeTable(1);
                        break;
                    case 3:
                        restaurant.BookTableCancelAsync(table);
                        break;
                    case 4:
                        restaurant.BookTableCancel(table);
                        break;
                    default:
                        break;
                }


                Console.WriteLine("Спасибо за ваше обращение!");
                stopWatch.Stop();
                var timeElapsed = stopWatch.Elapsed;

                Console.WriteLine($"{timeElapsed.Seconds:00}:{timeElapsed.Milliseconds:00}");

            }
        }
    }
}