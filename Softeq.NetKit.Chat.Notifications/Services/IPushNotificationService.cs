using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Notifications.PushNotifications;

namespace Softeq.NetKit.Chat.Notifications.Services
{
    public interface IPushNotificationService
    {
        Task SubscribeUserOnTagAsync(string userId, string tag);
        Task UnsubscribeUserFromTagAsync(string userId, string tag);
        Task<bool> SendToSingleAsync(string userId, IPushNotificationModel model);
        Task<bool> SendForTagAsync(IPushNotificationModel model, List<string> includedTags, List<string> excludedTags);
    }
}
