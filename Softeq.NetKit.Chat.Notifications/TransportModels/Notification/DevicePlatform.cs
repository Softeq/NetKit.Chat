using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DevicePlatform
    {
        iOS = 0,
        Android = 1
    }
}
