using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Services.PushNotifications.Models;

namespace Softeq.NetKit.Chat.Notifications.Services
{
    public interface IPushNotificationService
    {
        Task SubscribeUserOnTagAsync(string userId, string tagName);
        Task UnsubscribeUserFromTagAsync(string userId, string tagName);
        Task<bool> SendToSingleAsync(string userId, PushNotificationMessage model);
        Task<bool> SendForTagAsync(PushNotificationMessage model, List<string> includedTags, List<string> excludedTags);
    }
}
