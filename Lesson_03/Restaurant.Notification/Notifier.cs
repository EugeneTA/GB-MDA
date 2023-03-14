using Restaurant.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Notification
{
    public class Notifier
    {
        private readonly ConcurrentDictionary<Guid, Tuple<Guid?, Accepted>> _state = new();

        public void Accept(Guid orderId, Accepted accepted, Guid? clientId = null)
        {
            _state.AddOrUpdate(
                orderId,
                new Tuple<Guid?, Accepted>(clientId, accepted),
                (guid, oldValue) => new Tuple<Guid?, Accepted>(oldValue.Item1 ?? clientId, oldValue.Item2 | accepted)
                );
            Notify(orderId);
        }

        public void DishAccident(Guid orderId, Dish? dish)
        {
            foreach (var o in _state)
            {
                if (o.Key == orderId)
                {
                    Console.WriteLine($"Заказ {orderId} отменен. Гость {o.Value.Item1}, к сожалению, кухня не сможет приготовить {dish}.");
                    _state.Remove(orderId, out _);
                }
            }
        }

        //public void AcceptBooking(Guid orderId, Accepted accepted, Guid? clientId = null)
        //{
        //    Accept(orderId, accepted, clientId);
        //}

        //public void AcceptKitchen(Guid orderId, Accepted accepted, Guid? clientId = null)
        //{
        //    Accept(orderId, accepted, clientId);
        //    Notify(orderId);
        //}

        private void Notify(Guid orderId)
        {
            var booking = _state[orderId];

            switch (booking.Item2)
            {
                case Accepted.All:
                    {
                        Console.WriteLine($"Заказ {orderId} подтвержден. Столик успешно забронирован для гостя {booking.Item1}");
                        //_state.Remove(orderId, out _);
                    }
                    break;
                case Accepted.Rejected:
                    {
                        Console.WriteLine($"Заказ {orderId} отменен. Гость {booking.Item1}, к сожалению, все столики заняты");
                        _state.Remove(orderId, out _);
                    }
                    break;
                case Accepted.Kitchen:
                case Accepted.Booking:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();

            }
        }
    }
}
