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
                var sqlQuery = $@"
                    INSERT INTO Notifications
                    (
                        {nameof(Notification.Id)}, 
                        {nameof(Notification.IsRead)}, 
                        {nameof(Notification.MessageId)}, 
                        {nameof(Notification.ChannelId)}, 
                        {nameof(Notification.MemberId)}
                    ) VALUES 
                    (
                        @{nameof(Notification.Id)}, 
                        @{nameof(Notification.IsRead)}, 
                        @{nameof(Notification.MessageId)}, 
                        @{nameof(Notification.ChannelId)}, 
                        @{nameof(Notification.MemberId)}
                    )";

                await connection.ExecuteScalarAsync(sqlQuery, notification);
            }
        }

        public async Task DeletNotificationAsync(Guid notificationId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    DELETE FROM Notifications 
                    WHERE 
                        {nameof(Notification.Id)} = @{nameof(notificationId)}";

                await connection.ExecuteAsync(sqlQuery, new { notificationId });
            }
        }

        public async Task<Notification> GetNotificationByIdAsync(Guid notificationId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        {nameof(Notification.Id)}, 
                        {nameof(Notification.IsRead)}, 
                        {nameof(Notification.MessageId)}, 
                        {nameof(Notification.ChannelId)}, 
                        {nameof(Notification.MemberId)}
                    FROM 
                        Notifications
                    WHERE 
                        {nameof(Notification.Id)} = @{nameof(notificationId)}";

                return (await connection.QueryAsync<Notification>(sqlQuery, new { notificationId })).FirstOrDefault();
            }
        }

        public async Task<IReadOnlyCollection<Notification>> GetMemberNotificationsWithMemberMessageAndChannelAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        n.{nameof(Notification.Id)}, 
                        n.{nameof(Notification.IsRead)}, 
                        n.{nameof(Notification.MessageId)}, 
                        n.{nameof(Notification.ChannelId)}, 
                        n.{nameof(Notification.MemberId)},
                        m.{nameof(Message.Id)},
                        m.{nameof(Message.Body)},
                        m.{nameof(Message.Created)},
                        m.{nameof(Message.ImageUrl)},
                        m.{nameof(Message.Type)},
                        m.{nameof(Message.ChannelId)},
                        m.{nameof(Message.OwnerId)},
                        m.{nameof(Message.Updated)},
                        m.{nameof(Message.ForwardMessageId)},
                        m.{nameof(Message.AccessibilityStatus)},
                        me.{nameof(Member.Id)},
                        me.{nameof(Member.Email)},
                        me.{nameof(Member.IsAfk)},
                        me.{nameof(Member.IsBanned)},
                        me.{nameof(Member.LastActivity)},
                        me.{nameof(Member.LastNudged)},
                        me.{nameof(Member.Name)},
                        me.{nameof(Member.PhotoName)},
                        me.{nameof(Member.Role)},
                        me.{nameof(Member.SaasUserId)},
                        me.{nameof(Member.Status)},
                        me.{nameof(Member.IsActive)},
                        c.{nameof(Channel.Id)}, 
                        c.{nameof(Channel.Created)}, 
                        c.{nameof(Channel.Name)}, 
                        c.{nameof(Channel.CreatorId)}, 
                        c.{nameof(Channel.IsClosed)}, 
                        c.{nameof(Channel.MembersCount)}, 
                        c.{nameof(Channel.Type)}, 
                        c.{nameof(Channel.Description)}, 
                        c.{nameof(Channel.WelcomeMessage)}, 
                        c.{nameof(Channel.Updated)}, 
                        c.{nameof(Channel.PhotoUrl)}
                    FROM 
                        Notifications n
                    INNER JOIN Messages m 
                        ON n.{nameof(Notification.MessageId)} = m.{nameof(Message.Id)}
                    INNER JOIN Members me 
                        ON m.{nameof(Message.OwnerId)} = me.{nameof(Member.Id)}
                    INNER JOIN Channels c 
                        ON n.{nameof(Message.ChannelId)} = c.{nameof(Channel.Id)}
                    WHERE 
                        n.{nameof(Notification.MemberId)} = @{nameof(memberId)}";

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