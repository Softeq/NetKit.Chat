using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Settings
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotificationSettingKey
    {
        // Fill in according to the notification settings of your application
        TemplateLiked = 0
    }
}
