using Softeq.NetKit.Chat.Notifications.TransportModels.Shared.Request;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Request
{
    public class GetNotificationRequest : UserRequest
    {
        public GetNotificationRequest(string userId, string notificationId) : base(userId)
        {
            NotificationId = notificationId;
        }

        public string NotificationId { get; set; }
    }
}
