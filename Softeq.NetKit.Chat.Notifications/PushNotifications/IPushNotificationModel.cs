using System;
using System.Collections.Generic;
using System.Text;

namespace Softeq.NetKit.Chat.Notifications.PushNotifications
{
    public interface IPushNotificationModel
    {
        PushNotificationType NotificationType { get; set; }
        string Title { get; set; }
        string Body { get; set; }
        string Sound { get; set; }
        int Badge { get; set; }
        string GetData();
    }
}
