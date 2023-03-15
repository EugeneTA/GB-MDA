using Restaurant.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Booking.Models
{
    public class Table
    {
        public int Id { get; }
        public Order? Order { get; private set; }
        public State State { get; private set; }
        public int SeatsCount { get; }

        public Table(int id)
        {
            Id = id;
            State = State.Free;
            SeatsCount = new Random().Next(2, 5);
            Order = null;
        }

        public bool SetState(State state)
        {
            if (state == State) return false;

            if (state == State.Booked && State == State.Free)
            {
                State = state;
                return true;
            }

            State = state;
            ClearOrder();
            return true;
        }
        public void SetOrder(Order order)
        {
            Order = order;
        }
        public void ClearOrder()
        {
            Order = null;
        }

    }
}
