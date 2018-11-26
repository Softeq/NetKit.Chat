using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Notifications.PushNotifications.Model;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification;
using Softeq.NetKit.Chat.Notifications.TransportModels.PushNotification.Request;
using Softeq.NetKit.Chat.Notifications.TransportModels.Shared.Request;

namespace Softeq.NetKit.Chat.Notifications.Services
{
    public class PushNotificationService
    {
        private readonly INotificationHub _notificationHub;

        public PushNotificationService(INotificationHub notificationHub)
        {
            _notificationHub = notificationHub;
        }

        public async Task UnsubscribeDeviceFromPushAsync(CreatePushTokenRequest model)
        {
            await _notificationHub.DeleteRegistrationAsync(model.DeviceToken);
        }

        public async Task UnsubscribeUserFromPushAsync(UserRequest model)
        {
            await _notificationHub.DeleteAllRegistrationAsync(model.UserId);
        }

        public async Task CreateOrUpdatePushSubscriptionAsync(CreatePushTokenRequest model)
        {
            try
            {
                var tags = new List<string> { model.UserId };
                //TODO: Add role tags
                await _notificationHub.CreateOrUpdateRegistrationAsync(null, new DeviceRegistration()
                {
                    DeviceToken = model.DeviceToken,
                    Platform = (DevicePlatform)model.Platform,
                    Tags = tags.ToArray()
                });
            }
            catch (Exception ex)
            {
               throw new Exception($"Error while creating push notifications subscription, {ex}. Platform: {model.Platform}. Device token: {model.DeviceToken}. User id: {model.UserId}");
            }
        }

        public async Task<bool> SendToSingleAsync(string userId, IPushNotificationModel model)
        {
            try
            {
                model.RecipientIds = new[] { userId };
                return await _notificationHub.SendAsync(DevicePlatform.iOS, model);
            }
            catch (Exception ex)
            {
                //throw new Exception($"Error while sending push notifications. UserId: {userId}, NotificationType: {model.NotificationType}. Exception: \n {ex}");
                return false;
            }
        }
    }
}
