using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum NotificationSettingKey
    {
        GroupNotifications = 0
    }
}
