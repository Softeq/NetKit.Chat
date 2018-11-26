using System.Collections.Generic;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Response
{
    public class NotificationsResult
    {
        /// <summary>
        /// The size of this page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The records this page represents.
        /// </summary>
        public IEnumerable<NotificationResponse> Results { get; set; }
    }
}
