using Softeq.NetKit.Chat.Notifications.TransportModels.Shared.Request;


namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Request
{
    public class UpdateNotificationRequest : UserRequest
    {
        public UpdateNotificationRequest(string userId, string notificationId, dynamic notification) : base(userId)
        {
            NotificationId = notificationId;
            Notification = notification;
        }

        public string NotificationId { get; }
        public dynamic Notification { get; }
    }
}
