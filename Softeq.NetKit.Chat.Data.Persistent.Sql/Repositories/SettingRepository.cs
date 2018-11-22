// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Domain.Settings;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Repositories
{
    internal class SettingRepository : ISettingRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public SettingRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task AddSettingsAsync(Settings settings)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    INSERT INTO Settings(Id, RawSettings, ChannelId) 
                    VALUES (@Id, @RawSettings, @ChannelId);";
                
                await connection.ExecuteScalarAsync(sqlQuery, settings);
            }
        }

        public async Task<List<Settings>> GetAllSettingsAsync()
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, RawSettings, ChannelId 
                    FROM Settings";

                var data = (await connection.QueryAsync<Settings>(sqlQuery)).ToList();

                return data;
            }
        }

        public async Task DeleteSettingsAsync(Guid settingsId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"DELETE FROM Settings WHERE Id = @settingsId";

                await connection.ExecuteAsync(sqlQuery, new { settingsId });
            }
        }

        public async Task<Settings> GetSettingsByIdAsync(Guid settingsId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, RawSettings, ChannelId
                    FROM Settings
                    WHERE Id = @settingsId";
                
                var data = (await connection.QueryAsync<Settings>(sqlQuery, new { settingsId }))
                    .FirstOrDefault();
                
                return data;
            }
        }

        public async Task<Settings> GetSettingsByChannelIdAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, RawSettings, ChannelId
                    FROM Settings
                    WHERE ChannelId = @channelId";

                var data = (await connection.QueryAsync<Settings>(sqlQuery, new { channelId }))
                    .FirstOrDefault();

                return data;
            }
        }
    }
}