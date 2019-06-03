// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Dapper;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Domain.DomainModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Repositories
{
    internal class MessageRepository : BaseRepository, IMessageRepository
    {
        public MessageRepository(ISqlConnectionFactory sqlConnectionFactory) : base(sqlConnectionFactory)
        {
        }

        public async Task<Message> GetAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT
                       {nameof(Message.Id)},
                       {nameof(Message.Body)},
                       {nameof(Message.Created)},
                       {nameof(Message.ImageUrl)},
                       {nameof(Message.Type)},
                       {nameof(Message.ChannelId)},
                       {nameof(Message.OwnerId)},
                       {nameof(Message.Updated)},
                       {nameof(Message.ForwardMessageId)},
                       {nameof(Message.AccessibilityStatus)}
                    FROM 
                        Messages
                    WHERE 
                        {nameof(Message.Id)} = @{nameof(messageId)}";

                return await connection.QueryFirstAsync<Message>(sqlQuery, new { messageId });
            }
        }

        public async Task<IReadOnlyCollection<Message>> GetAllChannelMessagesWithOwnersAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT
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
                        me.{nameof(Member.IsBanned)},
                        me.{nameof(Member.LastActivity)},
                        me.{nameof(Member.LastNudged)},
                        me.{nameof(Member.Name)},
                        me.{nameof(Member.PhotoName)},
                        me.{nameof(Member.Role)},
                        me.{nameof(Member.SaasUserId)},
                        me.{nameof(Member.Status)},
                        me.{nameof(Member.IsActive)}
                    FROM 
                        Messages m
                    INNER JOIN Members me 
                        ON m.{nameof(Message.OwnerId)} = me.{nameof(Member.Id)}
                    WHERE 
                        m.{nameof(Message.ChannelId)} = @{nameof(channelId)}
                        AND m.{nameof(Message.AccessibilityStatus)} = accessibilityStatus
                    ORDER BY 
                        m.{nameof(Message.Created)} DESC";

                return (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.OwnerId = member.Id;
                            message.Owner = member;
                            message.AccessibilityStatus = AccessibilityStatus.Present;
                            return message;
                        },
                        new { channelId, accessibilityStatus = AccessibilityStatus.Present }))
                    .Distinct()
                    .ToList()
                    .AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<Message>> GetAllDirectChannelMessagesWithOwnersAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT
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
                        me.{nameof(Member.IsBanned)},
                        me.{nameof(Member.LastActivity)},
                        me.{nameof(Member.LastNudged)},
                        me.{nameof(Member.Name)},
                        me.{nameof(Member.PhotoName)},
                        me.{nameof(Member.Role)},
                        me.{nameof(Member.SaasUserId)},
                        me.{nameof(Member.Status)},
                        me.{nameof(Member.IsActive)}
                    FROM 
                        Messages m
                    INNER JOIN Members me 
                        ON m.{nameof(Message.OwnerId)} = me.{nameof(Member.Id)}
                    WHERE 
                        m.DirectChannelId = @{nameof(channelId)}
                        AND m.{nameof(Message.AccessibilityStatus)} = @accessibilityStatus
                    ORDER BY 
                        m.{nameof(Message.Created)} DESC";

                return (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.OwnerId = member.Id;
                            message.Owner = member;
                            message.AccessibilityStatus = AccessibilityStatus.Present;
                            return message;
                        },
                        new { channelId, accessibilityStatus = AccessibilityStatus.Present }))
                    .Distinct()
                    .ToList()
                    .AsReadOnly();
            }
        }

        public async Task<IReadOnlyList<Guid>> FindMessageIdsAsync(Guid channelId, string searchText)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var searchTerm = $"%{searchText.Trim().Replace("[", "[[]").Replace("%", "[%]").Replace("_", "[_]")}%";

                var sqlQuery = $@"
                    SELECT 
                        {nameof(Message.Id)}
                    FROM 
                        Messages m                    
                    WHERE 
                        {nameof(Message.ChannelId)} = @{nameof(channelId)} 
                        AND {nameof(Message.Body)} LIKE @{nameof(searchTerm)}
                        AND m.{nameof(AccessibilityStatus)} = @accessibilityStatus
                    ORDER BY 
                        m.{nameof(Message.Created)} DESC";

                return (await connection.QueryAsync<Guid>(sqlQuery, new { channelId, searchTerm, accessibilityStatus = AccessibilityStatus.Present })).ToList().AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<Message>> GetOlderMessagesWithOwnersAsync(Guid channelId, DateTimeOffset lastReadMessageCreated, int? pageSize)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT {(pageSize.HasValue ? $"TOP(@{nameof(pageSize)})" : "")} 
                        message.{nameof(Message.Id)},
                        message.{nameof(Message.Body)},
                        message.{nameof(Message.Created)},
                        message.{nameof(Message.ImageUrl)},
                        message.{nameof(Message.Type)},
                        message.{nameof(Message.ChannelId)},
                        message.{nameof(Message.OwnerId)},
                        message.{nameof(Message.Updated)},
                        message.{nameof(Message.ForwardMessageId)},
                        message.{nameof(Message.AccessibilityStatus)},
                        member.{nameof(Member.Id)},
                        member.{nameof(Member.Email)},
                        member.{nameof(Member.IsBanned)},
                        member.{nameof(Member.LastActivity)},
                        member.{nameof(Member.LastNudged)},
                        member.{nameof(Member.Name)},
                        member.{nameof(Member.PhotoName)},
                        member.{nameof(Member.Role)},
                        member.{nameof(Member.SaasUserId)},
                        member.{nameof(Member.Status)},
                        member.{nameof(Member.IsActive)}
                    FROM 
                        Messages message
                    INNER JOIN Members member 
                        ON message.{nameof(Message.OwnerId)} = member.{nameof(Member.Id)}
                    WHERE 
                        {nameof(Message.ChannelId)} = @{nameof(channelId)} 
                        AND {nameof(Message.Created)} < @{nameof(lastReadMessageCreated)} 
                        AND message.{nameof(Message.AccessibilityStatus)} = @accessibilityStatus
                    ORDER BY 
                        {nameof(Message.Created)} {(pageSize.HasValue ? "DESC" : "")}";


                return (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.OwnerId = member.Id;
                            message.Owner = member;
                            message.AccessibilityStatus = AccessibilityStatus.Present;
                            return message;
                        },
                        new { channelId, lastReadMessageCreated, pageSize, accessibilityStatus = AccessibilityStatus.Present }
                        ))
                    .Distinct()
                    .OrderBy(o => o.Created)
                    .ToList()
                    .AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<Message>> GetMessagesWithOwnersAsync(Guid channelId, DateTimeOffset lastReadMessageCreated, int? pageSize)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT {(pageSize.HasValue ? $"TOP(@{nameof(pageSize)})" : "")} 
                        message.{nameof(Message.Id)},
                        message.{nameof(Message.Body)},
                        message.{nameof(Message.Created)},
                        message.{nameof(Message.ImageUrl)},
                        message.{nameof(Message.Type)},
                        message.{nameof(Message.ChannelId)},
                        message.{nameof(Message.OwnerId)},
                        message.{nameof(Message.Updated)},
                        message.{nameof(Message.ForwardMessageId)},
                        message.{nameof(Message.AccessibilityStatus)},
                        member.{nameof(Member.Id)},
                        member.{nameof(Member.Email)},
                        member.{nameof(Member.IsBanned)},
                        member.{nameof(Member.LastActivity)},
                        member.{nameof(Member.LastNudged)},
                        member.{nameof(Member.Name)},
                        member.{nameof(Member.PhotoName)},
                        member.{nameof(Member.Role)},
                        member.{nameof(Member.SaasUserId)},
                        member.{nameof(Member.Status)},
                        member.{nameof(Member.IsActive)}
                    FROM 
                        Messages message
                    INNER JOIN Members member 
                        ON message.{nameof(Message.OwnerId)} = member.{nameof(Member.Id)}
                    WHERE
                        {nameof(Message.Created)} >= @{nameof(lastReadMessageCreated)}
                        AND {nameof(Message.ChannelId)} = @{nameof(channelId)} 
                        AND message.{nameof(Message.AccessibilityStatus)} = @accessibilityStatus
                    ORDER BY 
                        {nameof(Message.Created)}";

                return (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.OwnerId = member.Id;
                            message.Owner = member;
                            return message;
                        },
                        new { channelId, lastReadMessageCreated, pageSize, accessibilityStatus = AccessibilityStatus.Present }))
                    .Distinct()
                    .ToList()
                    .AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<Message>> GetLastMessagesWithOwnersAsync(Guid channelId, DateTimeOffset? lastReadMessageCreated, int readMessagesCount)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                // Take last readMessagesCount read messages
                var lastReadMessages = $@"
                    SELECT 
                        {nameof(Message.Id)}, 
                        {nameof(Message.Body)}, 
                        {nameof(Message.ChannelId)}, 
                        {nameof(Message.Created)}, 
                        {nameof(Message.ImageUrl)}, 
                        {nameof(Message.OwnerId)}, 
                        {nameof(Message.Type)}, 
                        {nameof(Message.Updated)},
                        memberId AS {nameof(Member.Id)}, 
                        {nameof(Member.Email)}, 
                        {nameof(Member.IsBanned)}, 
                        {nameof(Member.LastActivity)}, 
                        {nameof(Member.LastNudged)}, 
                        {nameof(Member.Name)}, 
                        {nameof(Member.PhotoName)}, 
                        {nameof(Member.Role)}, 
                        {nameof(Member.SaasUserId)}, 
                        {nameof(Member.Status)}
                    FROM 
                        (SELECT TOP (@{nameof(readMessagesCount)}) 
                            m.{nameof(Message.Id)}, 
                            m.{nameof(Message.Body)}, 
                            m.{nameof(Message.ChannelId)}, 
                            m.{nameof(Message.Created)}, 
                            m.{nameof(Message.ImageUrl)}, 
                            m.{nameof(Message.OwnerId)}, 
                            m.{nameof(Message.Type)}, 
                            m.{nameof(Message.Updated)},
                            me.{nameof(Member.Id)} AS memberId, 
                            me.{nameof(Member.Email)}, 
                            me.{nameof(Member.IsBanned)}, 
                            me.{nameof(Member.LastActivity)}, 
                            me.{nameof(Member.LastNudged)}, 
                            me.{nameof(Member.Name)}, 
                            me.{nameof(Member.PhotoName)}, 
                            me.{nameof(Member.Role)}, 
                            me.{nameof(Member.SaasUserId)}, 
                            me.{nameof(Member.Status)}
                        FROM 
                            Messages m  
                        INNER JOIN Members me 
                            ON m.{nameof(Message.OwnerId)} = me.{nameof(Member.Id)}
                        WHERE 
                            {nameof(Message.Created)} <= @{nameof(lastReadMessageCreated)}
                            AND {nameof(Message.ChannelId)} = @{nameof(channelId)}
                            AND m.{nameof(Message.AccessibilityStatus)} = @accessibilityStatus
                        ORDER BY 
                            {nameof(Message.Created)} DESC)
                    AS LastReadMessages";

                // Take all new messages
                var newMessages = $@"
                    SELECT 
                        {nameof(Message.Id)}, 
                        {nameof(Message.Body)}, 
                        {nameof(Message.ChannelId)}, 
                        {nameof(Message.Created)}, 
                        {nameof(Message.ImageUrl)}, 
                        {nameof(Message.OwnerId)}, 
                        {nameof(Message.Type)}, 
                        {nameof(Message.Updated)},
                        memberId AS {nameof(Member.Id)}, 
                        {nameof(Member.Email)}, 
                        {nameof(Member.IsBanned)}, 
                        {nameof(Member.LastActivity)}, 
                        {nameof(Member.LastNudged)}, 
                        {nameof(Member.Name)}, 
                        {nameof(Member.PhotoName)}, 
                        {nameof(Member.Role)}, 
                        {nameof(Member.SaasUserId)}, 
                        {nameof(Member.Status)}
                    FROM 
                        (SELECT 
                            m.{nameof(Message.Id)}, 
                            m.{nameof(Message.Body)}, 
                            m.{nameof(Message.ChannelId)}, 
                            m.{nameof(Message.Created)}, 
                            m.{nameof(Message.ImageUrl)}, 
                            m.{nameof(Message.OwnerId)}, 
                            m.{nameof(Message.Type)}, 
                            m.{nameof(Message.Updated)},
                            me.{nameof(Member.Id)} AS memberId, 
                            me.{nameof(Member.Email)}, 
                            me.{nameof(Member.IsBanned)}, 
                            me.{nameof(Member.LastActivity)}, 
                            me.{nameof(Member.LastNudged)}, 
                            me.{nameof(Member.Name)}, 
                            me.{nameof(Member.PhotoName)}, 
                            me.{nameof(Member.Role)}, 
                            me.{nameof(Member.SaasUserId)}, 
                            me.{nameof(Member.Status)}
                        FROM Messages m  
                        INNER JOIN Members me 
                            ON m.{nameof(Message.OwnerId)} = me.{nameof(Member.Id)}
                        WHERE 
                            {nameof(Message.Created)} > @{nameof(lastReadMessageCreated)}
                            AND {nameof(Message.ChannelId)} = @{nameof(channelId)} 
                            AND m.{nameof(Message.AccessibilityStatus)} = @accessibilityStatus) 
                    AS NewMessages";

                var sqlQuery = $"{lastReadMessages} UNION {newMessages} ORDER BY {nameof(Message.Created)}";

                return (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.OwnerId = member.Id;
                            message.Owner = member;
                            return message;
                        },
                        new
                        {
                            channelId,
                            lastReadMessageCreated,
                            readMessagesCount,
                            accessibilityStatus = AccessibilityStatus.Present
                        }))
                    .Distinct()
                    .ToList()
                    .AsReadOnly();
            }
        }

        public async Task<Message> GetMessageWithOwnerAndForwardMessageAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT
                        Messages.{nameof(Message.Id)},
                        Messages.{nameof(Message.Body)},
                        Messages.{nameof(Message.Created)},
                        Messages.{nameof(Message.ImageUrl)},
                        Messages.{nameof(Message.Type)},
                        Messages.{nameof(Message.ChannelId)},
                        Messages.{nameof(Message.OwnerId)},
                        Messages.{nameof(Message.Updated)},
                        Messages.{nameof(Message.ForwardMessageId)},
                        Messages.{nameof(Message.AccessibilityStatus)},
                        Members.{nameof(Member.Id)},
                        Members.{nameof(Member.Email)},
                        Members.{nameof(Member.IsBanned)},
                        Members.{nameof(Member.LastActivity)},
                        Members.{nameof(Member.LastNudged)},
                        Members.{nameof(Member.Name)},
                        Members.{nameof(Member.PhotoName)},
                        Members.{nameof(Member.Role)},
                        Members.{nameof(Member.SaasUserId)},
                        Members.{nameof(Member.Status)},
                        Members.{nameof(Member.IsActive)},
                        ForwardMessages.{nameof(ForwardMessage.Id)},
                        ForwardMessages.{nameof(ForwardMessage.Body)},
                        ForwardMessages.{nameof(ForwardMessage.Created)},
                        ForwardMessages.{nameof(ForwardMessage.ChannelId)},
                        ForwardMessages.{nameof(ForwardMessage.OwnerId)}
                    FROM 
                        Messages 
                    INNER JOIN Members 
                        ON Messages.{nameof(Message.OwnerId)} = Members.{nameof(Member.Id)}
                    LEFT JOIN ForwardMessages 
                        ON Messages.{nameof(Message.ForwardMessageId)} = ForwardMessages.{nameof(ForwardMessage.Id)}
                    WHERE 
                        Messages.Id = @{nameof(messageId)} 
                        AND Messages.{nameof(Message.AccessibilityStatus)} = @accessibilityStatus";

                return (await connection.QueryAsync<Message, Member, ForwardMessage, Message>(
                        sqlQuery,
                        (message, member, forwardMessage) =>
                        {
                            if (forwardMessage != null)
                            {
                                message.ForwardMessageId = forwardMessage.Id;
                                message.ForwardedMessage = forwardMessage;
                            }
                            message.Owner = member;
                            message.OwnerId = member.Id;
                            return message;
                        },
                        new { messageId, accessibilityStatus = AccessibilityStatus.Present }))
                     .FirstOrDefault();
            }
        }

        public async Task<Message> GetPreviousMessageAsync(Guid? channelId, DateTimeOffset created)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT TOP(1) 
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
                        mem.{nameof(Member.Id)},
                        mem.{nameof(Member.Email)},
                        mem.{nameof(Member.IsBanned)},
                        mem.{nameof(Member.LastActivity)},
                        mem.{nameof(Member.LastNudged)},
                        mem.{nameof(Member.Name)},
                        mem.{nameof(Member.PhotoName)},
                        mem.{nameof(Member.Role)},
                        mem.{nameof(Member.SaasUserId)},
                        mem.{nameof(Member.Status)},
                        mem.{nameof(Member.IsActive)}
		            FROM 
                        Messages m    
		            LEFT JOIN Members mem 
                        ON mem.{nameof(Member.Id)} = m.{nameof(Message.OwnerId)}
		            WHERE 
                        m.{nameof(Message.ChannelId)} = @{nameof(channelId)} 
                        AND m.{nameof(Message.Created)} < @{nameof(created)} 
                        AND m.{nameof(Message.AccessibilityStatus)} = @accessibilityStatus
		            ORDER BY
                        {nameof(Message.Created)} DESC";

                return (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (msg, member) =>
                        {
                            msg.Owner = member;
                            msg.OwnerId = member.Id;
                            return msg;
                        },
                        new { channelId, created, accessibilityStatus = AccessibilityStatus.Present }))
                    .FirstOrDefault();
            }
        }

        public async Task AddMessageAsync(Message message)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    INSERT INTO Messages
                    (
                        {nameof(Message.Id)}, 
                        {nameof(Message.Body)}, 
                        {nameof(Message.Created)}, 
                        {nameof(Message.ImageUrl)}, 
                        {nameof(Message.Type)}, 
                        {nameof(Message.ChannelId)}, 
                        {nameof(Message.OwnerId)}, 
                        {nameof(Message.Updated)}, 
                        {nameof(Message.ForwardMessageId)}, 
                        {nameof(Message.AccessibilityStatus)}
                    ) VALUES 
                    (
                        @{nameof(Message.Id)}, 
                        @{nameof(Message.Body)}, 
                        @{nameof(Message.Created)}, 
                        @{nameof(Message.ImageUrl)}, 
                        @{nameof(Message.Type)}, 
                        @{nameof(Message.ChannelId)}, 
                        @{nameof(Message.OwnerId)}, 
                        @{nameof(Message.Updated)}, 
                        @{nameof(Message.ForwardMessageId)}, 
                        @{nameof(Message.AccessibilityStatus)}
                    )";

                await connection.ExecuteScalarAsync(sqlQuery, message);
            }
        }

        public async Task ArchiveMessageAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    UPDATE Messages
                    SET 
                        {nameof(Message.AccessibilityStatus)} = @accessibilityStatus
                    WHERE 
                        {nameof(Message.Id)} = @{nameof(messageId)}";

                await connection.ExecuteAsync(sqlQuery, new { messageId, accessibilityStatus = AccessibilityStatus.Archived });
            }
        }

        public async Task UpdateMessageBodyAsync(Guid messageId, string body, DateTimeOffset updated)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    UPDATE Messages 
                    SET 
                        {nameof(Message.Body)} = @{nameof(body)},
                        {nameof(Message.Updated)} = @{nameof(updated)}
                    WHERE 
                        {nameof(Message.Id)} = @{nameof(messageId)}";

                await connection.ExecuteAsync(sqlQuery, new { messageId, body, updated });
            }
        }

        public async Task<int> GetChannelMessagesCountAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        COUNT(*) 
                    FROM 
                        Messages 
                    WHERE 
                        {nameof(Message.ChannelId)} = @{nameof(channelId)} 
                        AND Messages.{nameof(Message.AccessibilityStatus)} = @accessibilityStatus";

                return (await connection.QueryAsync<int>(sqlQuery, new { channelId, accessibilityStatus = AccessibilityStatus.Present })).FirstOrDefault();
            }
        }

        public async Task<Message> GetLastReadMessageAsync(Guid memberId, Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
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
                        mem.{nameof(Member.Id)},
                        mem.{nameof(Member.Email)},
                        mem.{nameof(Member.IsBanned)},
                        mem.{nameof(Member.LastActivity)},
                        mem.{nameof(Member.LastNudged)},
                        mem.{nameof(Member.Name)},
                        mem.{nameof(Member.PhotoName)},
                        mem.{nameof(Member.Role)},
                        mem.{nameof(Member.SaasUserId)},
                        mem.{nameof(Member.Status)},
                        mem.{nameof(Member.IsActive)},
                        fm.{nameof(ForwardMessage.Id)},
                        fm.{nameof(ForwardMessage.Body)},
                        fm.{nameof(ForwardMessage.Created)},
                        fm.{nameof(ForwardMessage.ChannelId)},
                        fm.{nameof(ForwardMessage.OwnerId)}
                    FROM 
                        Messages m 
                    INNER JOIN ChannelMembers c 
                        ON m.{nameof(Message.Id)} = c.{nameof(ChannelMember.LastReadMessageId)}
                    INNER JOIN Members mem 
                        ON c.{nameof(ChannelMember.MemberId)} = mem.{nameof(Member.Id)}
                    LEFT JOIN ForwardMessages fm 
                        ON m.{nameof(Message.ForwardMessageId)} = fm.{nameof(ForwardMessage.Id)}
                    WHERE 
                        c.{nameof(ChannelMember.MemberId)} = @{nameof(memberId)} 
                        AND c.{nameof(ChannelMember.ChannelId)} = @{nameof(channelId)} 
                        AND m.{nameof(Message.AccessibilityStatus)} = @accessibilityStatus";

                return (await connection.QueryAsync<Message, Member, ForwardMessage, Message>(
                        sqlQuery,
                        (message, member, forwardedMessage) =>
                        {
                            message.ForwardedMessage = forwardedMessage;
                            message.Owner = member;
                            message.OwnerId = member.Id;
                            return message;
                        },
                        new { memberId, channelId, accessibilityStatus = AccessibilityStatus.Present },
                        splitOn: "Id, Id, Id"))
                    .FirstOrDefault();
            }
        }
    }
}