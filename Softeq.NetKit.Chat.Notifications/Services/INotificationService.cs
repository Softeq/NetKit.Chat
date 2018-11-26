using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Name.Service.TransportModels.Setting.Response;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Request;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Response;
using Softeq.NetKit.Chat.Notifications.TransportModels.Setting.Request;
using Softeq.NetKit.Chat.Notifications.TransportModels.Shared.Request;

namespace Softeq.NetKit.Chat.Notifications.Services
{
    public interface INotificationService
    {
        Task<NotificationsResult> GetUserNotificationsAsync(GetNotificationsRequest request);
        Task<NotificationResponse> GetNotificationByIdAsync(GetNotificationRequest request);
        Task PostNotificationAsync(CreateNotificationRequest request);
        Task RemoveNotificationsAsync(UserRequest request);
        Task UpdateNotificationAsync(UpdateNotificationRequest request);
    }
}
