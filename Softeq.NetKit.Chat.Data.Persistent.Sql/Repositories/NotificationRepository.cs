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
    internal class NotificationRepository : INotificationRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public NotificationRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"INSERT INTO Notifications(Id, IsRead, MessageId, ChannelId, MemberId) 
                                 VALUES (@Id, @IsRead, @MessageId, @ChannelId, @MemberId)";

                await connection.ExecuteScalarAsync(sqlQuery, notification);
            }
        }

        public async Task DeletNotificationAsync(Guid notificationId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"DELETE FROM Notifications 
                                 WHERE Id = @notificationId";

                await connection.ExecuteAsync(sqlQuery, new { notificationId });
            }
        }

        public async Task<Notification> GetNotificationByIdAsync(Guid notificationId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM Notifications
                                 WHERE Id = @notificationId";

                return (await connection.QueryAsync<Notification>(sqlQuery, new { notificationId })).FirstOrDefault();
            }
        }

        public async Task<IReadOnlyCollection<Notification>> GetMemberNotificationsWithMemberMessageAndChannelAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT * 
                                 FROM Notifications n
                                 INNER JOIN Messages m ON n.MessageId = m.Id
                                 INNER JOIN Members me ON m.OwnerId = me.Id
                                 INNER JOIN Channels c ON n.ChannelId = c.Id
                                 WHERE n.MemberId = @memberId";

                return (await connection.QueryAsync<Notification, Message, Member, Channel, Notification>(
                        sqlQuery,
                        (notification, message, member, channel) =>
                        {
                            notification.Channel = channel;
                            notification.ChannelId = channel.Id;
                            notification.Message = message;
                            notification.MessageId = message.Id;
                            notification.Member = member;
                            notification.MemberId = member.Id;
                            return notification;
                        },
                        new { memberId }))
                    .Distinct()
                    .ToList()
                    .AsReadOnly();
            }
        }
    }
}