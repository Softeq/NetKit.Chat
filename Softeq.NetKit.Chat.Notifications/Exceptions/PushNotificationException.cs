using System;
using System.Collections.Generic;
using System.Text;

namespace Softeq.NetKit.Chat.Notifications.Exceptions
{
    public class PushNotificationException : System.Exception
    {
        public PushNotificationException(string message)
            : base(message)
        {
        }

        public PushNotificationException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
