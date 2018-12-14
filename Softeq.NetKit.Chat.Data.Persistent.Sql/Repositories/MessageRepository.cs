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
    internal class MessageRepository : IMessageRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public MessageRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<IReadOnlyCollection<Message>> GetAllChannelMessagesWithOwnersAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM Messages m
                                 INNER JOIN Members me ON m.OwnerId = me.Id
                                 WHERE m.ChannelId = @channelId
                                 ORDER BY m.Created DESC";

                return (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.OwnerId = member.Id;
                            message.Owner = member;
                            return message;
                        },
                        new { channelId }))
                    .Distinct()
                    .ToList()
                    .AsReadOnly();
            }
        }

        public async Task<IReadOnlyList<Guid>> FindMessageIdsAsync(Guid channelId, string searchText)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT Id
                                 FROM Messages m                    
                                 WHERE ChannelId = @channelId AND Body LIKE @searchTerm
                                 ORDER BY m.Created DESC";

                var searchTerm = $"%{searchText.Trim().Replace("[", "[[]").Replace("%", "[%]").Replace("_","[_]")}%";
                return (await connection.QueryAsync<Guid>(sqlQuery, new { channelId, searchTerm })).ToList().AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<Message>> GetOlderMessagesWithOwnersAsync(Guid channelId, DateTimeOffset lastReadMessageCreated, int? pageSize)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                string sqlQuery;

                if (pageSize.HasValue)
                {
                    sqlQuery = @"SELECT TOP(@pageSize) *
                                 FROM Messages message
                                 INNER JOIN Members member ON message.OwnerId = member.Id
                                 WHERE ChannelId = @channelId AND Created < @lastReadMessageCreated
                                 ORDER BY Created DESC";
                }
                else
                {
                    sqlQuery = @"SELECT *
                                 FROM Messages message
                                 INNER JOIN Members member ON message.OwnerId = member.Id
                                 WHERE ChannelId = @channelId AND Created < @lastReadMessageCreated
                                 ORDER BY Created";
                }

                return (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.OwnerId = member.Id;
                            message.Owner = member;
                            return message;
                        },
                        new { channelId, lastReadMessageCreated, pageSize }
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
                var pageSizeQuery = pageSize == null ? "" : "TOP(@pageSize)";
                var sqlQuery = $"SELECT {pageSizeQuery} * " +
                               @"FROM Messages message
                                 INNER JOIN Members member ON message.OwnerId = member.Id
                                 WHERE Created >= @lastReadMessageCreated AND ChannelId = @channelId 
                                 ORDER BY Created";

                return (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.OwnerId = member.Id;
                            message.Owner = member;
                            return message;
                        },
                        new { ChannelId = channelId, LastReadMessageCreated = lastReadMessageCreated, PageSize = pageSize }))
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
                var lastReadMessages = @"SELECT Id, Body, ChannelId, Created, ImageUrl, OwnerId, Type, Updated,
							                    memberId AS Id, Email, IsAfk, IsBanned, LastActivity, LastNudged, Name, PhotoName, Role, SaasUserId, Status 
                                         FROM (
                                             SELECT TOP (@ReadMessagesCount) m.Id, m.Body, m.ChannelId, m.Created, m.ImageUrl, m.OwnerId, m.Type, m.Updated,
							                                                 me.Id AS memberId, me.Email, me.IsAfk, me.IsBanned, me.LastActivity, me.LastNudged, me.Name, me.PhotoName, me.Role, me.SaasUserId, me.Status 
							                 FROM Messages m  
                                             INNER JOIN Members me ON m.OwnerId = me.Id
                                             WHERE Created <= @LastReadMessageCreated AND ChannelId = @ChannelId
					                         ORDER BY Created DESC) 
                                         AS LastReadMessages";

                // Take all new messages
                var newMessages = @"SELECT * FROM (
                                        SELECT m.Id, m.Body, m.ChannelId, m.Created, m.ImageUrl, m.OwnerId, m.Type, m.Updated,
							                   me.Id AS memberId, me.Email, me.IsAfk, me.IsBanned, me.LastActivity, me.LastNudged, me.Name, me.PhotoName, me.Role, me.SaasUserId, me.Status 
							            FROM Messages m  
                                        INNER JOIN Members me ON m.OwnerId = me.Id
                                        WHERE Created > @LastReadMessageCreated AND ChannelId = @ChannelId) 
                                    AS NewMessages";

                var sqlQuery = $"{lastReadMessages} UNION {newMessages} ORDER BY Created";

                return (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.OwnerId = member.Id;
                            message.Owner = member;
                            return message;
                        },
                        new { ChannelId = channelId, LastReadMessageCreated = lastReadMessageCreated, ReadMessagesCount = readMessagesCount }))
                    .Distinct()
                    .ToList()
                    .AsReadOnly();
            }
        }
        
        public async Task<Message> GetMessageWithOwnerAndForwardMessageAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM Messages m 
                                 INNER JOIN Members mem ON m.OwnerId = mem.Id
                                 LEFT JOIN ForwardMessages fm ON m.ForwardMessageId = fm.Id
                                 WHERE m.Id = @messageId";

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
                        new { messageId }))
                     .FirstOrDefault();
            }
        }

        public async Task<Message> GetPreviousMessageAsync(Guid channelId, Guid? ownerId, DateTimeOffset created)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT TOP(1) *
		                         FROM Messages m    
		                         LEFT JOIN Members mem ON mem.Id = m.OwnerId   
		                         WHERE m.ChannelId = @channelId AND OwnerId = @ownerId AND m.Created < @created
		                         ORDER BY Created ASC";

                return (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (msg, member) =>
                        {
                            msg.Owner = member;
                            msg.OwnerId = member.Id;
                            return msg;
                        },
                        new { channelId, ownerId, created }))
                    .FirstOrDefault();
            }
        }

        public async Task AddMessageAsync(Message message)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"INSERT INTO Messages(Id, Body, Created, ImageUrl, Type, ChannelId, OwnerId, Updated, ForwardMessageId) 
                                 VALUES (@Id, @Body, @Created, @ImageUrl, @Type, @ChannelId, @OwnerId, @Updated, @ForwardMessageId)";

                await connection.ExecuteScalarAsync(sqlQuery, message);
            }
        }

        public async Task DeleteMessageAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"DELETE FROM Messages 
                                 WHERE Id = @messageId";

                await connection.ExecuteAsync(sqlQuery, new { messageId });
            }
        }

        public async Task UpdateMessageBodyAsync(Guid messageId, string body, DateTimeOffset updated)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"UPDATE Messages 
                                 SET Body = @body, Updated = @updated
                                 WHERE Id = @messageId";

                await connection.ExecuteAsync(sqlQuery, new { messageId, body, updated });
            }
        }

        public async Task<int> GetChannelMessagesCountAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT COUNT(*) 
                                 FROM Messages 
                                 Where ChannelId = @channelId";

                return (await connection.QueryAsync<int>(sqlQuery, new { channelId })).FirstOrDefault();
            }
        }

        public async Task<Message> GetLastReadMessageAsync(Guid memberId, Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM Messages m 
                                 INNER JOIN ChannelMembers c ON m.Id = c.LastReadMessageId
                                 INNER JOIN Members mem ON c.MemberId = mem.Id
                                 LEFT JOIN ForwardMessages  fm ON m.ForwardMessageId = fm.Id
                                 WHERE c.MemberId = @memberId AND c.ChannelId = @channelId";

                return (await connection.QueryAsync<Message, Member, ForwardMessage, Message>(
                        sqlQuery,
                        (message, member, forwardedMessage) =>
                        {
                            message.ForwardedMessage = forwardedMessage;
                            message.Owner = member;
                            message.OwnerId = member.Id;
                            return message;
                        },
                        new { memberId, channelId },
                        splitOn: "Id, ChannelId, Id"))
                    .FirstOrDefault();
            }
        }
    }
}