using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
    public interface INotificationSettingRepository
    {
        Task<NotificationSettings> GetSettingsByMemberIdAsync(Guid memberId);
        Task AddSettingsAsync(NotificationSettings settings);
        Task UpdateSettingsAsync(NotificationSettings settings);
        Task<IList<Guid>> GetSaasUserIdsWithDisabledGroupNotificationsAsync();
    }
}
