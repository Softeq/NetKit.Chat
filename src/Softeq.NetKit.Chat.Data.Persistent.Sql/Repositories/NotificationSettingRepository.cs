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
    internal class NotificationSettingRepository : INotificationSettingRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public NotificationSettingRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<NotificationSettings> GetSettingsByMemberIdAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        {nameof(NotificationSettings.Id)}, 
                        {nameof(NotificationSettings.MemberId)}, 
                        {nameof(NotificationSettings.IsChannelNotificationsDisabled)}
                    FROM
                        NotificationSettings
                    WHERE
                        {nameof(NotificationSettings.MemberId)} = @{nameof(memberId)}";

                return (await connection.QueryAsync<NotificationSettings>(sqlQuery, new { memberId })).FirstOrDefault();
            }
        }

        public async Task AddSettingsAsync(NotificationSettings settings)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    INSERT INTO NotificationSettings 
                    (
                        {nameof(NotificationSettings.Id)}, 
                        {nameof(NotificationSettings.MemberId)}, 
                        {nameof(NotificationSettings.IsChannelNotificationsDisabled)}
                    ) VALUES 
                    (
                        @{nameof(NotificationSettings.Id)}, 
                        @{nameof(NotificationSettings.MemberId)}, 
                        @{nameof(NotificationSettings.IsChannelNotificationsDisabled)}
                    )";

                await connection.ExecuteScalarAsync(sqlQuery, settings);
            }
        }

        public async Task UpdateSettingsAsync(NotificationSettings settings)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    UPDATE NotificationSettings 
                    SET 
                        {nameof(NotificationSettings.MemberId)} = @{nameof(NotificationSettings.MemberId)},
                        {nameof(NotificationSettings.IsChannelNotificationsDisabled)} = @{nameof(NotificationSettings.IsChannelNotificationsDisabled)}
                    WHERE 
                        {nameof(NotificationSettings.Id)} = @{nameof(NotificationSettings.Id)}";

                await connection.ExecuteScalarAsync(sqlQuery, settings);
            }
        }

        public async Task<IList<string>> GetSaasUserIdsWithDisabledGroupNotificationsAsync()
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        {nameof(Member.SaasUserId)}
                    FROM
                        NotificationSettings
                    INNER JOIN Members
                        ON Members.{nameof(Member.Id)} = NotificationSettings.{nameof(NotificationSettings.MemberId)}
                    WHERE
                        {nameof(NotificationSettings.IsChannelNotificationsDisabled)} = 1";

                return (await connection.QueryAsync<string>(sqlQuery)).ToList().AsReadOnly();
            }
        }
    }
}
