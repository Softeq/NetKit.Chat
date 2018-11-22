// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
    public interface ISettingRepository
    {
        Task AddSettingsAsync(Settings settings);
        Task<List<Settings>> GetAllSettingsAsync();
        Task DeleteSettingsAsync(Guid settingsId);
        Task<Settings> GetSettingsByIdAsync(Guid settingsId);
        Task<Settings> GetSettingsByChannelIdAsync(Guid channelId);
    }
}