using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.NotificationHubs;
using Softeq.NetKit.Chat.Notifications.PushNotifications.Model;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification;

namespace Softeq.NetKit.Chat.Notifications
{
    public interface INotificationHub
    {
        Task<IEnumerable<string>> GetAllRegistrations(string tag);
        Task<IEnumerable<RegistrationDescription>> GetRegistrationsByTokenAsync(string deviceToken);
        Task DeleteRegistrationAsync(string deviceToken);
        Task DeleteAllRegistrationAsync(string tag);
        Task CreateOrUpdateRegistrationAsync(string registrationId, DeviceRegistration deviceUpdate);
        Task<bool> SendAsync(DevicePlatform platform, IPushNotificationModel model);
    }
}
