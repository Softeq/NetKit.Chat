// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Domain.Channel;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Domain.Message;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Repositories
{
    internal class ChannelRepository : IChannelRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public ChannelRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<List<Channel>> GetAllChannelsAsync()
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, Created, IsClosed, CreatorId, MembersCount, Updated, Name, Type, Description, WelcomeMessage, PhotoUrl 
                    FROM Channels";

                var data = (await connection.QueryAsync<Channel>(sqlQuery)).ToList();
                
                return data;
            }
        }

        // TODO: Improve performance
        public async Task<List<Channel>> GetAllowedChannelsAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT c.Id, c.Created, c.Name, c.CreatorId, c.IsClosed, c.MembersCount, c.Type, c.Description, c.WelcomeMessage, c.Updated, c.PhotoUrl,
                    m.Id, m.ChannelId, m.Created, m.Body, m.ImageUrl, m.Type, m.OwnerId, m.Updated, 
                    mem.Id, mem.SaasUserId, mem.Name, mem.Status, mem.Role, mem.IsAfk, mem.Email, mem.LastActivity
                    FROM Channels c LEFT JOIN Messages m ON c.Id = m.ChannelId
                    LEFT JOIN Members mem ON m.OwnerId = mem.Id
                    WHERE (c.Id IN (SELECT cm.ChannelId FROM ChannelMembers cm 
                    WHERE cm.MemberId = @memberId)) AND c.IsClosed != 1
                    ORDER BY m.Created DESC";

                var channelDictionary = new Dictionary<Guid, Channel>();

                var data = (await connection.QueryAsync<Channel, Message, Member, Channel>(
                        sqlQuery,
                        (channel, message, member) =>
                        {
                            Channel channelEntry;
                            if (!channelDictionary.TryGetValue(channel.Id, out channelEntry))
                            {
                                channelEntry = channel;
                                channelEntry.Messages = new List<Message>();
                                channelDictionary.Add(channelEntry.Id, channelEntry);
                            }

                            if (message != null)
                            {
                                if (member != null)
                                {
                                    message.Owner = member;
                                }
                                channelEntry.Messages.Add(message);
                            }
                            return channelEntry;
                        },
                        new { memberId }))
                    .Distinct()
                    .ToList();

                return data;
            }
        }

        public async Task<Channel> GetChannelByNameAsync(string channelName)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT c.Id, c.Created, c.Name, c.CreatorId, c.IsClosed, c.MembersCount, c.Type, c.Description, c.WelcomeMessage, c.Updated, c.PhotoUrl,
                    m.Id, m.SaasUserId, m.Name, m.Status, m.Role, m.IsAfk, m.Email, m.LastActivity
                    FROM Channels c LEFT JOIN Members m ON c.CreatorId = m.Id
                    WHERE c.Name = @channelName";

                var data = (await connection.QueryAsync<Channel, Member, Channel>(
                        sqlQuery,
                        (channel, member) =>
                        {
                            channel.Creator = member;
                            channel.CreatorId = member.Id;
                            return channel;
                        },
                        new { channelName }))
                    .FirstOrDefault();

                return data;
            }
        }

        public async Task<Channel> GetChannelByIdAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT c.Id, c.Created, c.Name, c.CreatorId, c.IsClosed, c.MembersCount, c.Type, c.Description, c.WelcomeMessage, c.Updated, c.PhotoUrl,
                    m.Id, m.SaasUserId, m.Name, m.Status, m.Role, m.IsAfk, m.Email, m.LastActivity
                    FROM Channels c LEFT JOIN Members m ON c.CreatorId = m.Id
                    WHERE c.Id = @channelId";

                var data = (await connection.QueryAsync<Channel, Member, Channel>(
                        sqlQuery,
                        (channel, member) =>
                        {
                            channel.Creator = member;
                            channel.CreatorId = member.Id;
                            return channel;
                        },
                        new { channelId }))
                    .FirstOrDefault();

                return data;
            }
        }

        public async Task<bool> CheckIfMemberExistInChannelAsync(Guid memberId, Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT CASE WHEN EXISTS (
                        SELECT *
                        FROM Members m
	                    INNER JOIN ChannelMembers c ON m.Id = c.MemberId
                        WHERE c.MemberId = @memberId AND c.ChannelId = @channelId
                    )
                    THEN CAST(1 AS BIT)
                    ELSE CAST(0 AS BIT) END";

                var data = (await connection.QueryAsync<bool>(sqlQuery, new { memberId, channelId }))
                    .FirstOrDefault();

                return data;
            }
        }

        public async Task AddChannelAsync(Channel channel)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    INSERT INTO Channels(Id, Created, IsClosed, CreatorId, MembersCount, Updated, Name, Type, Description, WelcomeMessage, PhotoUrl) 
                    VALUES (@Id, @Created, @IsClosed, @CreatorId, @MembersCount, @Updated, @Name, @Type, @Description, @WelcomeMessage, @PhotoUrl);";

                await connection.ExecuteScalarAsync(sqlQuery, channel);
            }
        }

        public async Task DeleteChannelAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"DELETE FROM Channels WHERE Id = @channelId";

                await connection.ExecuteAsync(sqlQuery, new { channelId });
            }
        }

        public async Task UpdateChannelAsync(Channel channel)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"UPDATE Channels 
                                 SET Name = @Name, 
                                     Description = @Description, 
                                     WelcomeMessage = @WelcomeMessage, 
                                     MembersCount = @MembersCount,
                                     Type = @Type, 
                                     IsClosed = @IsClosed, 
                                     Updated = @Updated,
                                     PhotoUrl = @PhotoUrl
                                 WHERE Id = @Id";

                
                await connection.ExecuteAsync(sqlQuery, channel);
            }
        }

        public async Task<List<Channel>> GetChannelsByMemberId(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT * FROM Channels c
                    WHERE c.CreatorId = @memberId AND c.IsClosed != 1";

                var data = (await connection.QueryAsync<Channel>(sqlQuery, new { memberId })).ToList();

                return data;
            }
        }

        public async Task IncrementChannelMembersCount(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"UPDATE Channels
                                SET MembersCount = MembersCount + 1
                                WHERE Id = @channelId";

                await connection.ExecuteAsync(sqlQuery, new { channelId });
            }
        }

        public async Task DecrementChannelMembersCount(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"UPDATE Channels
                                SET MembersCount = MembersCount - 1
                                WHERE Id = @channelId";
                
                await connection.ExecuteAsync(sqlQuery, new { channelId });
            }
        }
    }
}