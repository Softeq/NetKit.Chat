using System;
using System.Collections.Generic;
using System.Text;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Request
{
    public class GetNotificationsRequest
    {
        public GetNotificationsRequest(string userId, int take)
        {
            CurrentUserId = userId;
            Take = take;
        }

        public string CurrentUserId { get; }
        public int Take { get; }
    }
}
