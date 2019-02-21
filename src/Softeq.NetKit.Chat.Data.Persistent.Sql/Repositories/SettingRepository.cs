// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Repositories
{
    internal class SettingRepository : ISettingRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public SettingRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task AddSettingsAsync(Settings settings)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"INSERT INTO Settings(Id, RawSettings, ChannelId) 
                                 VALUES (@Id, @RawSettings, @ChannelId)";

                await connection.ExecuteScalarAsync(sqlQuery, settings);
            }
        }

        public async Task<IReadOnlyCollection<Settings>> GetAllSettingsAsync()
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT Id, RawSettings, ChannelId 
                                 FROM Settings";

                return (await connection.QueryAsync<Settings>(sqlQuery)).ToList().AsReadOnly();
            }
        }

        public async Task DeleteSettingsAsync(Guid settingsId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"DELETE FROM Settings 
                                 WHERE Id = @settingsId";

                await connection.ExecuteAsync(sqlQuery, new { settingsId });
            }
        }

        public async Task<Settings> GetSettingsByIdAsync(Guid settingsId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT Id, RawSettings, ChannelId
                                 FROM Settings
                                 WHERE Id = @settingsId";

                return (await connection.QueryAsync<Settings>(sqlQuery, new { settingsId })).FirstOrDefault();
            }
        }

        public async Task<Settings> GetSettingsByChannelIdAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT Id, RawSettings, ChannelId
                                 FROM Settings
                                 WHERE ChannelId = @channelId";

                return (await connection.QueryAsync<Settings>(sqlQuery, new { channelId })).FirstOrDefault();
            }
        }
    }
}