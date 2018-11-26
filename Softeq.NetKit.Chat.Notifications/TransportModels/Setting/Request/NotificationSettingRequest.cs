using System;
using System.Collections.Generic;
using System.Text;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Settings;
using Softeq.NetKit.Chat.Notifications.TransportModels.Shared.Request;

namespace Softeq.NoName.Service.TransportModels.Setting.Request
{
    public class NotificationSettingRequest : UserRequest
    {
        public NotificationSettingRequest(string userId) : base(userId)
        {
        }

        public NotificationSettingKey Key { get; set; }
        public NotificationSettingValue Value { get; set; }
    }
}
