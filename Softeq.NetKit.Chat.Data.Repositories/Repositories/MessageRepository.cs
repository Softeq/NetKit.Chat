// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Softeq.NetKit.Chat.Data.Interfaces.Repository;
using Softeq.NetKit.Chat.Data.Repositories.Infrastructure;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Domain.Message;

namespace Softeq.NetKit.Chat.Data.Repositories.Repositories
{
    internal class MessageRepository : IMessageRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public MessageRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<List<Message>> GetAllChannelMessagesAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT *
                    FROM Messages m
                    INNER JOIN Members me ON m.OwnerId = me.Id
                    WHERE m.ChannelId = @channelId
                    ORDER BY m.Created DESC";

                var data = (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.Owner = member;
                            return message;
                        },
                        new { channelId }))
                    .Distinct()
                    .ToList();

                return data;          
            }
        }

        public async Task<List<Message>> GetOlderMessagesAsync(Guid channelId, DateTimeOffset lastReadMessageCreated, int? pageSize)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                
                var sqlQuery = pageSize != null ?
                    @"
                    SELECT * FROM 
					(SELECT TOP(@pageSize) m.Id, m.Body, m.ChannelId, m.Created, m.ImageUrl, m.OwnerId, m.Type, m.Updated,
				    me.Id AS memberId, me.Email, me.IsAfk, me.IsBanned, me.LastActivity, me.LastNudged, me.Name, me.PhotoName, me.Role, me.SaasUserId, me.Status 
                    FROM Messages m
                    INNER JOIN Members me ON m.OwnerId = me.Id
                    WHERE Created < @LastReadMessageCreated AND ChannelId = @ChannelId
                    ORDER BY Created DESC) AS temp
					ORDER BY temp.Created" :
                    @"
                    SELECT *
                    FROM Messages m
                    INNER JOIN Members me ON m.OwnerId = me.Id
                    WHERE Created < @LastReadMessageCreated AND ChannelId = @ChannelId 
                    ORDER BY Created";

                var data = (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.Owner = member;
                            return message;
                        },
                        new { ChannelId = channelId, LastReadMessageCreated = lastReadMessageCreated, PageSize = pageSize }))
                    .Distinct()
                    .OrderBy(x => x.Created)
                    .ToList();

                return data;
            }
        }

        public async Task<List<Message>> GetMessagesAsync(Guid channelId, DateTimeOffset lastReadMessageCreated, int? pageSize)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = pageSize != null ?
                    @"
					SELECT TOP (@pageSize) *
                    FROM Messages m
                    INNER JOIN Members me ON m.OwnerId = me.Id
                    WHERE Created >= @LastReadMessageCreated AND ChannelId = @ChannelId
                    ORDER BY Created" :
                    @"
                    SELECT *
                    FROM Messages m
                    INNER JOIN Members me ON m.OwnerId = me.Id
                    WHERE Created >= @LastReadMessageCreated AND ChannelId = @ChannelId 
                    ORDER BY Created";

                var data = (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.Owner = member;
                            return message;
                        },
                        new { ChannelId = channelId, LastReadMessageCreated = lastReadMessageCreated, PageSize = pageSize }))
                    .Distinct()
                    .OrderBy(x => x.Created)
                    .ToList();

                return data;
            }
        }

        public async Task<List<Message>> GetLastMessagesAsync(Guid channelId, DateTimeOffset? lastReadMessageCreated, int readMessagesCount = 20)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = lastReadMessageCreated != null ?
                    @"
                    SELECT Id, Body, ChannelId, Created, ImageUrl, OwnerId, Type, Updated,
							memberId AS Id, Email, IsAfk, IsBanned, LastActivity, LastNudged, Name, PhotoName, Role, SaasUserId, Status 
                    FROM
                        (SELECT TOP (@ReadMessagesCount) 
                            m.Id, m.Body, m.ChannelId, m.Created, m.ImageUrl, m.OwnerId, m.Type, m.Updated,
							me.Id AS memberId, me.Email, me.IsAfk, me.IsBanned, me.LastActivity, me.LastNudged, me.Name, me.PhotoName, me.Role, me.SaasUserId, me.Status 
							FROM Messages m  
                            INNER JOIN Members me ON m.OwnerId = me.Id
                            WHERE Created <= @LastReadMessageCreated AND ChannelId = @ChannelId
					        ORDER BY Created DESC) AS LastReadMessages
					    UNION
					SELECT * FROM
                        (SELECT m.Id, m.Body, m.ChannelId, m.Created, m.ImageUrl, m.OwnerId, m.Type, m.Updated,
							me.Id AS memberId, me.Email, me.IsAfk, me.IsBanned, me.LastActivity, me.LastNudged, me.Name, me.PhotoName, me.Role, me.SaasUserId, me.Status 
							FROM Messages m  
                            INNER JOIN Members me ON m.OwnerId = me.Id
                            WHERE Created > @LastReadMessageCreated AND ChannelId = @ChannelId) AS NewMessages
                    ORDER BY Created " :
                    @"
                    SELECT *
                    FROM Messages m
                    INNER JOIN Members me ON m.OwnerId = me.Id
                    WHERE ChannelId = @ChannelId
                    ORDER BY Created";

                var data = (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.Owner = member;
                            return message;
                        },
                        new { ChannelId = channelId, LastReadMessageCreated = lastReadMessageCreated, ReadMessagesCount = readMessagesCount }))
                    .Distinct()
                    .OrderBy(x => x.Created)
                    .ToList();

                return data;
            }
        }

        public async Task<List<Message>> GetPreviousMessagesAsync(Guid channelId, Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT *
                    FROM Messages m
                    INNER JOIN Members me ON m.OwnerId = me.Id
                    WHERE m.ChannelId = @channelId AND 
                    (SELECT Created FROM Messages WHERE Id = @messageId) > m.Created";

                var data = (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.Owner = member;
                            return message;
                        },
                        new { messageId, channelId }))
                    .Distinct()
                    .ToList();

                return data;
            }
        }

        public async Task<Message> GetMessageByIdAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT *
                    FROM Messages m INNER JOIN Members mem ON m.OwnerId = mem.Id
                    WHERE m.Id = @messageId";

                var data = (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.Owner = member;
                            message.OwnerId = member.Id;
                            return message;
                        },
                        new { messageId }))
                        .FirstOrDefault();
                
                return data;
            }
        }

        public async Task<Message> GetPreviousMessageAsync(Message message)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                var sqlQuery = @"
		            SELECT TOP(1) *
		            FROM Messages m    
		            LEFT JOIN Members mem ON mem.Id = m.OwnerId   
		            WHERE m.ChannelId = @channelId AND OwnerId = @ownerId AND m.Created < @createdDate
		            ORDER BY Created ASC";

                var previousMessage = (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (msg, member) =>
                        {
                            msg.Owner = member;
                            msg.OwnerId = member.Id;
                            return msg;
                        },
                        new { channelId = message.ChannelId, ownerId = message.OwnerId, createdDate = message.Created }))
                    .FirstOrDefault();
                return previousMessage;
            }
        }

        public async Task AddMessageAsync(Message message)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    INSERT INTO Messages(Id, Body, Created, ImageUrl, Type, ChannelId, OwnerId) 
                    VALUES (@Id, @Body, @Created, @ImageUrl, @Type, @ChannelId, @OwnerId)";

                await connection.ExecuteScalarAsync(sqlQuery, message);
            }
        }

        public async Task DeleteMessageAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"DELETE FROM Messages WHERE Id = @messageId";
                
                await connection.ExecuteAsync(sqlQuery, new { messageId });
            }
        }

        public async Task UpdateMessageAsync(Message message)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"UPDATE Messages 
                                 SET Body = @Body, Updated = @Updated
                                 WHERE Id = @Id";
                
                await connection.ExecuteAsync(sqlQuery, message);
            }
        }

        public async Task<int> GetChannelMessagesCountAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT COUNT(*) FROM Messages 
                    Where ChannelId = @channelId";

                var data = (await connection.QueryAsync<int>(sqlQuery, new { channelId })).FirstOrDefault();
            
                return data;
            }
        }

        public async Task<Message> GetLastReadMessageAsync(Guid memberId, Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT *
                    FROM Messages m 
                    INNER JOIN ChannelMembers c ON m.Id = c.LastReadMessageId
                    INNER JOIN Members mem ON c.MemberId = mem.Id
                    WHERE c.MemberId = @memberId AND c.ChannelId = @channelId";

                var data = (await connection.QueryAsync<Message, Member, Message>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.Owner = member;
                            message.OwnerId = member.Id;
                            return message;
                        },
                        new { memberId, channelId },
                        splitOn: "Id, ChannelId, Id"))
                    .FirstOrDefault();

                return data;
            }
        }
    }
}
