using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Softeq.NetKit.Chat.Notifications.PushNotifications;

namespace Softeq.NetKit.Chat.Notifications.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        public PushNotificationService()
        {
            
        }

        public async Task SubscribeUserOnTagAsync(string userId, string tag)
        {
            throw new NotImplementedException();
        }

        public async Task UnsubscribeUserFromTagAsync(string userId, string tag)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SendToSingleAsync(string tag, IPushNotificationModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SendForTagAsync(IPushNotificationModel model, List<string> includedTags, List<string> excludedTags)
        {
            throw new NotImplementedException();
        }
    }
}
