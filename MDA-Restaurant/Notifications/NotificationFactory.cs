using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDA_Restaurant.Notifications
{
    public static class NotificationFactory
    {
        public static INotification GetNotificationMethod(NotificationType notificationType)
        {
            switch (notificationType)
            {
                case NotificationType.Phone: 
                    return new PhoneNotification();
                case NotificationType.SMS:
                    return new SMSNotification();
                default:
                    throw new ArgumentException("No such notification type");
            }
        }
    }
}
