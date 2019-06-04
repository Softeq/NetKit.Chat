using Softeq.NetKit.Chat.Domain.TransportModels.Request;
using Softeq.NetKit.Services.PushNotifications.Models;

namespace Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Request
{
    public class CreatePushTokenRequest : UserRequest
    {
        public CreatePushTokenRequest(string userId) : base(userId)
        {
        }

        public string Token { get; set; }

        public PushPlatformEnum DevicePlatform { get; set; }
    }
}
