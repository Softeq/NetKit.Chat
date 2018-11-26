using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Settings
{
    [JsonConverter(typeof(StringEnumConverter))]
    public class NotificationSetting : Entity<Guid>
    {
        // Fill in according to the notification settings of your application
        public NotificationSettingValue TemplateLiked { get; set; } = NotificationSettingValue.True;

        public string SaasUserId { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}
