using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Settings;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Settings;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface INotificationSettingsService
    {
        Task<IList<Guid>> GetSaasUserIdsWithDisabledGroupNotificationsAsync();
        Task<NotificationSettingResponse> UpdateUserNotificationSettingsAsync(NotificationSettingRequest notificationSettingRequest);
        Task<NotificationSettingResponse> GetUserNotificationSettingsAsync(UserRequest userRequest);
    }
}
