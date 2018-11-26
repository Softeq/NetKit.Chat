using Softeq.NetKit.Chat.Notifications.TransportModels.Notification;
using Softeq.NetKit.Chat.Notifications.TransportModels.Shared.Request;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.PushNotification.Request
{
    public class CreatePushTokenRequest : UserRequest
    {
        public CreatePushTokenRequest(string userId, DevicePlatform platform, string deviceToken) : base(userId)
        {
            Platform = platform;
            DeviceToken = deviceToken;
        }

        public DevicePlatform Platform { get; }
        public string DeviceToken { get; }
    }
}
