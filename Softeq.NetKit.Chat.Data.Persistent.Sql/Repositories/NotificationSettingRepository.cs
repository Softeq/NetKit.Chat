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
                var sqlQuery = @"SELECT Id, MemberId, IsChannelNotificationsDisabled 
                                 FROM   NotificationSettings
                                 WHERE  MemberId = @memberId";

                return (await connection.QueryAsync<NotificationSettings>(sqlQuery, new { memberId })).FirstOrDefault();
            }
        }

        public async Task AddSettingsAsync(NotificationSettings settings)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"INSERT INTO NotificationSettings (Id, MemberId, IsChannelNotificationsDisabled) 
                                 VALUES (@Id, @MemberId, @IsChannelNotificationsDisabled)";

                await connection.ExecuteScalarAsync(sqlQuery, settings);
            }
        }

        public async Task UpdateSettingsAsync(NotificationSettings settings)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"UPDATE NotificationSettings 
                                 SET    MemberId = @MemberId,
                                        IsChannelNotificationsDisabled = @IsChannelNotificationsDisabled
                                 WHERE  Id = @Id";

                await connection.ExecuteScalarAsync(sqlQuery, settings);
            }
        }

        public async Task<IList<string>> GetSaasUserIdsWithDisabledGroupNotificationsAsync()
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT SaasUserId 
                                 FROM NotificationSettings
                                 INNER JOIN Members
                                 ON Members.Id = NotificationSettings.MemberId
                                 WHERE  IsChannelNotificationsDisabled = 1";

                return (await connection.QueryAsync<string>(sqlQuery)).ToList().AsReadOnly();
            }
        }
    }
}
