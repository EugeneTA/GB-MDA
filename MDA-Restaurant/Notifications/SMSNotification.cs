﻿using MDA_Restaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDA_Restaurant.Notifications
{
    public class SMSNotification : INotification
    {
        public void BookingNotification(Table table)
        {
            Console.WriteLine(table is null
                ? $"УВЕДОМЛЕНИЕ: Извините, но сейчас все столики заняты."
                : $"УВЕДОМЛЕНИЕ: Готово! Для вас забронирован столик номер {table.Id}.");
        }

        public void CancelBookingNotification(int tableIdToCancel, Table table)
        {
            Console.WriteLine(table is null
                ? $"УВЕДОМЛЕНИЕ: Извините, но столик с номером {tableIdToCancel} не был забронирован."
                : $"УВЕДОМЛЕНИЕ: Готово! Бронь для столика номер {table.Id} успешно аннулирована.");
        }
    }
}
