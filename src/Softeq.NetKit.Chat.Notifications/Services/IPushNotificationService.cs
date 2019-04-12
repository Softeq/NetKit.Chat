using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Services.PushNotifications.Models;

namespace Softeq.NetKit.Chat.Notifications.Services
{
    public interface IPushNotificationService
    {
        Task SubscribeUserOnTagsAsync(string userId, IEnumerable<string> tagNames);
        Task SubscribeUserOnTagAsync(string userId, string tag);
        Task UnsubscribeUserFromTagAsync(string userId, string tag);
        Task<bool> SendToSingleAsync(string userId, PushNotificationMessage model);
        Task<bool> SendForTagAsync(PushNotificationMessage model, List<string> includedTags, List<string> excludedTags);
    }
}
