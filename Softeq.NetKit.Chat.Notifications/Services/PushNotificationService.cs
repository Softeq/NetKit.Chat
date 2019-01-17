using Softeq.NetKit.Chat.Notifications.PushNotifications;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Notifications.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        public PushNotificationService()
        {

        }

        // TODO implementation.
        public async Task SubscribeUserOnTagAsync(string userId, string tag)
        {
            Task completedTask = Task.CompletedTask;
        }

        // TODO implementation.
        public async Task UnsubscribeUserFromTagAsync(string userId, string tag)
        {
            Task completedTask = Task.CompletedTask;
        }

        // TODO implementation.
        public async Task<bool> SendToSingleAsync(string tag, IPushNotificationModel model)
        {
            return true;
        }

        // TODO implementation.
        public async Task<bool> SendForTagAsync(IPushNotificationModel model, List<string> includedTags, List<string> excludedTags)
        {
            return true;
        }
    }
}
