namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Request
{
    public class CreateNotificationRequest
    {
        public CreateNotificationRequest(NotificationType notificationType, dynamic notification)
        {
            NotificationType = notificationType;
            Notification = notification;
        }

        public NotificationType NotificationType { get; set; }
        public dynamic Notification { get; set; }
    }
}
