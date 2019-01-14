using System;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Response
{
    public class NotificationResponse
    {
        public NotificationResponse(NotificationType type)
        {
            Type = type;
        }

        public Guid Id { get; set; }

        public DateTime Created { get; set; }

        public string OwnerUserId { get; set; }

        public NotificationType Type { get; }
    }
}
