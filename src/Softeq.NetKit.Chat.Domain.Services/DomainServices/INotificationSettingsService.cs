using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Settings;
using Softeq.NetKit.Chat.Domain.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Settings;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface INotificationSettingsService
    {
        Task<IList<string>> GetSaasUserIdsWithDisabledGroupNotificationsAsync();
        Task<NotificationSettingResponse> UpdateUserNotificationSettingsAsync(NotificationSettingRequest notificationSettingRequest);
        Task<NotificationSettingResponse> GetUserNotificationSettingsAsync(UserRequest userRequest);
    }
}
