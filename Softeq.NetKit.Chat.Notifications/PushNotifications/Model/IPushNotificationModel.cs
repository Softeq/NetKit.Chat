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

    public class PushNotificationModel : IPushNotificationModel
    {
        public int NotificationType { get; set; }
        public IEnumerable<string> RecipientIds { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int Badge { get; set; }

        public string GetData()
        {
            return "wazzz";
        }
    }
}
