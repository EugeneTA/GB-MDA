using MDA_Restaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MDA_Restaurant
{
    public class Restaurant
    {
        private readonly List<Table> _tables;

        public Restaurant()
        {
            _tables = new List<Table>();
            for (int i = 1; i <= 10; i++)
            {
                _tables.Add(new Table(i));
            }
        }

        public void BookFreeTable(int countOfPersons)
        {
            Console.WriteLine("Добрый день! Подождите секунду, я подберу столик и подтвержу вашу бронь. Оставайтесь на линии.");

            var table = _tables.FirstOrDefault(t => t.SeatsCount >= countOfPersons && t.State == State.Free);
            table?.SetState(State.Booked);

            Thread.Sleep(1000 * 5);

            Console.WriteLine(table is null
                ? $"Извините, но сейчас все столики заняты."
                : $"Готово! Для вас забронирован столик номер {table.Id}.");
        }

        public void BookFreeTableAsync(int countOfPersons)
        {
            Console.WriteLine("Добрый день! Подождите секунду, я подберу столик и подтвержу вашу бронь. Вам придет уведомление.");

            Task.Run(async () =>
            {
                var table = _tables.FirstOrDefault(t => t.SeatsCount >= countOfPersons && t.State == State.Free);
                table?.SetState(State.Booked);

                await Task.Delay(1000 * 5);

                Console.WriteLine(table is null
                    ? $"УВЕДОМЛЕНИЕ: Извините, но сейчас все столики заняты."
                    : $"УВЕДОМЛЕНИЕ: Готово! Для вас забронирован столик номер {table.Id}.");

            });
        }

        public void BookTableCancel(int tableId)
        {
            Console.WriteLine("Добрый день! Подождите секунду, я проверю вашу бронь. Оставайтесь на линии.");

            var table = _tables.FirstOrDefault(t => t.Id >= tableId  && t.State == State.Booked);
            table?.SetState(State.Free);

            Thread.Sleep(1000 * 5);

            Console.WriteLine(table is null
                ? $"Извините, но столик с номером {tableId} не был забронирован."
                : $"Готово! Бронь для столика номер {table.Id} успешно аннулирована.");
        }

        public void BookTableCancelAsync(int tableId)
        {
            Console.WriteLine("Добрый день! Подождите секунду, я проверю вашу бронь. Вам придет уведомление.");

            Task.Run(async () =>
            {
                var table = _tables.FirstOrDefault(t => t.Id >= tableId && t.State == State.Booked);
                table?.SetState(State.Free);

                await Task.Delay(1000 * 5);

                Console.WriteLine(table is null
                    ? $"УВЕДОМЛЕНИЕ: Извините, но столик с номером {tableId} не был забронирован."
                    : $"УВЕДОМЛЕНИЕ: Готово! Бронь для столика номер {table.Id} успешно аннулирована.");
            });

        }
    }
}
