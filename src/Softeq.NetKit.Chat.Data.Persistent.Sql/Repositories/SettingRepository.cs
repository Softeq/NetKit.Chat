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
                var sqlQuery = $@"
                    INSERT INTO Settings
                    (
                        {nameof(Settings.Id)}, 
                        {nameof(Settings.RawSettings)}, 
                        {nameof(Settings.ChannelId)}
                    ) VALUES 
                    (
                        @{nameof(Settings.Id)}, 
                        @{nameof(Settings.RawSettings)}, 
                        @{nameof(Settings.ChannelId)}
                    )";

                await connection.ExecuteScalarAsync(sqlQuery, settings);
            }
        }

        public async Task<IReadOnlyCollection<Settings>> GetAllSettingsAsync()
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        {nameof(Settings.Id)}, 
                        {nameof(Settings.RawSettings)}, 
                        {nameof(Settings.ChannelId)}
                    FROM 
                        Settings";

                return (await connection.QueryAsync<Settings>(sqlQuery)).ToList().AsReadOnly();
            }
        }

        public async Task DeleteSettingsAsync(Guid settingsId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    DELETE FROM Settings 
                    WHERE 
                        {nameof(Settings.Id)} = @{nameof(settingsId)}";

                await connection.ExecuteAsync(sqlQuery, new { settingsId });
            }
        }

        public async Task<Settings> GetSettingsByIdAsync(Guid settingsId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        {nameof(Settings.Id)}, 
                        {nameof(Settings.RawSettings)}, 
                        {nameof(Settings.ChannelId)}
                    FROM 
                        Settings
                    WHERE 
                        {nameof(Settings.Id)} = @{nameof(settingsId)}";

                return (await connection.QueryAsync<Settings>(sqlQuery, new { settingsId })).FirstOrDefault();
            }
        }

        public async Task<Settings> GetSettingsByChannelIdAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        {nameof(Settings.Id)}, 
                        {nameof(Settings.RawSettings)}, 
                        {nameof(Settings.ChannelId)} 
                    FROM 
                        Settings
                    WHERE 
                        {nameof(Settings.ChannelId)} = @{nameof(channelId)}";

                return (await connection.QueryAsync<Settings>(sqlQuery, new { channelId })).FirstOrDefault();
            }
        }
    }
}