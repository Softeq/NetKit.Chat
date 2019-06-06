using Softeq.NetKit.Services.PushNotifications.Models;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Request
{
    public class PushTokenRequest
    {
        public PushTokenRequest(string token, PushPlatformEnum devicePlatform)
        {
            Token = token;
            DevicePlatform = devicePlatform;
        }

        public string Token { get; set; }

        public PushPlatformEnum DevicePlatform { get; set; }
    }
}
