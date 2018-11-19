// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Softeq.NetKit.Chat.Data.Repositories.Infrastructure;
using Softeq.NetKit.Chat.Domain.Channel;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Domain.Message;
using Softeq.NetKit.Chat.Domain.Notification;

namespace Softeq.NetKit.Chat.Data.Repositories.Repositories
{
    internal class NotificationRepository : INotificationRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public NotificationRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    INSERT INTO Notifications(Id, IsRead, MessageId, ChannelId, MemberId) 
                    VALUES (@Id, @IsRead, @MessageId, @ChannelId, @MemberId);";
                
                await connection.ExecuteScalarAsync(sqlQuery, notification);
            }
        }

        public async Task DeletNotificationAsync(Guid notificationId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"DELETE FROM Notifications WHERE Id = @notificationId";
                
                await connection.ExecuteAsync(sqlQuery, new { notificationId }); 
            }
        }

        public async Task<Notification> GetNotificationByIdAsync(Guid notificationId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, IsRead, MessageId, ChannelId, MemberId 
                    FROM Notifications
                    WHERE Id = @notificationId";

                var data = (await connection.QueryAsync<Notification>(sqlQuery, new { notificationId }))
                    .FirstOrDefault();

                return data;
            }
        }

        public async Task<List<Notification>> GetMemberNotificationsAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT * 
                    FROM Notifications n
                    INNER JOIN Messages m ON n.MessageId = m.Id
                    INNER JOIN Members me ON m.OwnerId = me.Id
                    INNER JOIN Channels c ON n.ChannelId = c.Id
                    WHERE n.MemberId = @memberId";

                var data = (await connection.QueryAsync<Notification, Message, Member, Channel, Notification>(
                        sqlQuery,
                        (notification, message, member, channel) =>
                        {
                            notification.Channel = channel;
                            notification.Message = message;
                            notification.Message.Owner = member;
                            return notification;
                        },
                        new { memberId }))
                    .Distinct()
                    .ToList();

                return data;
            }
        }
    }
}