using MDA_Restaurant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDA_Restaurant.Notifications
{
    public interface INotification
    {
        public void BookingNotification(Table table);
        public void CancelBookingNotification(int tableIdToCancel,Table table);
    }
}
