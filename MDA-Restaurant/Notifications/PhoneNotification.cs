using MDA_Restaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDA_Restaurant.Notifications
{
    public class PhoneNotification : INotification
    {
        public void BookingNotification(Table table)
        {
            Console.WriteLine(table is null
                ? $"Извините, но сейчас все столики заняты."
                : $"Готово! Для вас забронирован столик номер {table.Id}.");
        }

        public void CancelBookingNotification(int tableIdToCancel, Table table)
        {
            Thread.Sleep(1000 * 5);
            Console.WriteLine(table is null
                ? $"Извините, но столик с номером {tableIdToCancel} не был забронирован."
                : $"Готово! Бронь для столика номер {table.Id} успешно аннулирована.");
        }
    }
}
