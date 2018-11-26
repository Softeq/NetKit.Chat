using System;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Response
{
    public abstract class NotificationResponse
    {
        protected NotificationResponse(NotificationType type)
        {
            Type = type;
        }

        public Guid Id { get; set; }

        public DateTime Created { get; set; }

        public string OwnerUserId { get; set; }

        public NotificationType Type { get; }
    }
}
