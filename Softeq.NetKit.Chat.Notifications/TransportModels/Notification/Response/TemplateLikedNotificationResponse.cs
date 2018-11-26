using Softeq.NetKit.Chat.Notifications.TransportModels.Notification;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Response
{
    public class NewMessageNotificationResponse : TemplateNotificationResponse
    {
        public NewMessageNotificationResponse(NotificationType type) : base(type)
        {
        }

        public string UserIdWhoLiked { get; set; }
    }
}
