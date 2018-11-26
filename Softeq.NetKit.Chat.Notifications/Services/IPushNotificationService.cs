using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Notifications.PushNotifications.Model;
using Softeq.NetKit.Chat.Notifications.TransportModels;
using Softeq.NetKit.Chat.Notifications.TransportModels.PushNotification.Request;
using Softeq.NoName.Service.TransportModels.Setting.Request;
using Softeq.NoName.Service.TransportModels.Setting.Response;

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
