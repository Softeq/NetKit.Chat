using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Settings
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotificationSettingValue
    {
        False = 0,
        True = 1
    }
}
