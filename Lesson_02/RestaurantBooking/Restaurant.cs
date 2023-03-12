using Messaging;
using RabbitMQ.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantBooking
{
    public class Restaurant
    {
        private readonly List<Table> _tables;
        private readonly Producer _producer;

        public Restaurant(Producer producer)
        {
            if (producer == null) throw new ArgumentNullException(nameof(producer));


            _tables = new List<Table>();
            for (int i = 1; i <= 10; i++)
            {
                _tables.Add(new Table(i));
            }

            _producer = producer;
        }


        public void BookFreeTableAsync(string exchangeName, string exchangeType, int countOfPersons)
        {
            Console.WriteLine("Добрый день! Подождите секунду, я подберу столик и подтвержу вашу бронь. Вам придет уведомление.");

            Task.Run(async () =>
            {
                Table? table = null;

                await Task.Delay(1000 * 5);

                lock (_tables)
                {
                    table = _tables.FirstOrDefault(t => t.SeatsCount >= countOfPersons && t.State == State.Free);
                    table?.SetState(State.Booked);
                }

                _producer?.Send(exchangeName, exchangeType, table is null
                    ? $"УВЕДОМЛЕНИЕ: Извините, но сейчас все столики заняты."
                    : $"УВЕДОМЛЕНИЕ: Готово! Для вас забронирован столик номер {table.Id}.");

            });
        }


        public void BookTableCancelAsync(string exchangeName, string exchangeType, int tableId)
        {
            Console.WriteLine("Добрый день! Подождите секунду, я проверю вашу бронь. Вам придет уведомление.");

            Task.Run(async () =>
            {
                Table? table = null;

                await Task.Delay(1000 * 5);

                lock (_tables)
                {
                    table = _tables.FirstOrDefault(t => t.Id >= tableId && t.State == State.Booked);
                    table?.SetState(State.Free);
                }

                _producer?.Send(exchangeName, exchangeType, table is null
                    ? $"УВЕДОМЛЕНИЕ: Извините, но столик с номером {tableId} не был забронирован."
                    : $"УВЕДОМЛЕНИЕ: Готово! Бронь для столика номер {table.Id} успешно аннулирована.");

            });

        }

        public void CancelAllBookings(Object stateInfo)
        {
            TimerStateObject? state = stateInfo as TimerStateObject;
            if (state != null)
            {
                lock (_tables)
                {
                    _tables.ForEach(t =>
                    {
                        if (t.State == State.Booked)
                        {
                            BookTableCancelAsync(state.ExchangeNameParam, state.ExchangeTypeParam, t.Id);
                        }
                        //t.SetState(State.Free);
                    });
                }

                Console.WriteLine("*** Все резервирования отменены *** ");
            }

        }
    }
}
