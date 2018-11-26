using System.Collections.Generic;


namespace Softeq.NetKit.Chat.Notifications.PushNotifications.Model
{
    public interface IPushNotificationModel
    {
        int NotificationType { get; set; }
        IEnumerable<string> RecipientIds { get; set; }
        string Title { get; set; }
        string Body { get; set; }
        int Badge { get; set; }
        string GetData();
    }
}
