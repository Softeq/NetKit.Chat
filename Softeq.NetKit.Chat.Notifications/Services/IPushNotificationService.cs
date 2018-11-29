using System.Threading.Tasks;
using Softeq.NetKit.Chat.Notifications.PushNotifications.Model;
using Softeq.NetKit.Chat.Notifications.TransportModels.PushNotification.Request;
using Softeq.NetKit.Chat.Notifications.TransportModels.Shared.Request;

namespace Softeq.NetKit.Chat.Notifications.Services
{
    public interface IPushNotificationService
    {
        Task UnsubscribeDeviceFromPushAsync(CreatePushTokenRequest model);
        Task UnsubscribeUserFromPushAsync(UserRequest model);
        Task CreateOrUpdatePushSubscriptionAsync(CreatePushTokenRequest model);
        Task<bool> SendToSingleAsync(string userId, IPushNotificationModel model);
    }
}
