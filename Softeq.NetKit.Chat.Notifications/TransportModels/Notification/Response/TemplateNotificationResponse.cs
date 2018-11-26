using System;
using System.Collections.Generic;
using System.Text;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Response;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Response
{
    public class TemplateNotificationResponse : NotificationResponse
    {
        public TemplateNotificationResponse(NotificationType type) : base(type)
        {
        }

        public string TemplateTitle { get; set; }
        public Guid TemplateId { get; set; }
    }
}
