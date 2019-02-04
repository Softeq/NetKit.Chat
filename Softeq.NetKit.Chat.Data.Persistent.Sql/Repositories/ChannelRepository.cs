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
    internal class ChannelRepository : IChannelRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public ChannelRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<IReadOnlyCollection<Channel>> GetAllChannelsAsync()
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM Channels";

                return (await connection.QueryAsync<Channel>(sqlQuery)).ToList().AsReadOnly();
            }
        }
        
        // TODO: Improve performance or split this method
        public async Task<IReadOnlyCollection<Channel>> GetAllowedChannelsWithMessagesAndCreatorAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT c.Id, c.Created, c.Name, c.CreatorId, c.IsClosed, c.MembersCount, c.Type, c.Description, c.WelcomeMessage, c.Updated, c.PhotoUrl,
                                        creator.Id, creator.SaasUserId, creator.Name, creator.Status, creator.Role, creator.IsAfk, creator.Email, creator.LastActivity,
                                        m.Id, m.ChannelId, m.Created, m.Body, m.ImageUrl, m.Type, m.OwnerId, m.Updated, 
                                        mem.Id, mem.SaasUserId, mem.Name, mem.Status, mem.Role, mem.IsAfk, mem.Email, mem.LastActivity
                                 FROM Channels c 
                                 LEFT JOIN Members creator ON c.CreatorId = creator.Id
                                 LEFT JOIN Messages m ON c.Id = m.ChannelId
                                 LEFT JOIN Members mem ON m.OwnerId = mem.Id
                                 WHERE (c.Id IN 
                                           (SELECT cm.ChannelId 
                                            FROM ChannelMembers cm 
                                            WHERE cm.MemberId = @memberId)) 
                                     AND c.IsClosed <> 1
                                 ORDER BY m.Created DESC";

                var channelDictionary = new Dictionary<Guid, Channel>();

                return (await connection.QueryAsync<Channel, Member, Message, Member, Channel>(
                        sqlQuery,
                        (channel, creator, message, member) =>
                        {
                            if (!channelDictionary.TryGetValue(channel.Id, out var channelEntry))
                            {
                                channelEntry = channel;
                                channelEntry.Messages = new List<Message>();
                                channelDictionary.Add(channelEntry.Id, channelEntry);
                            }
                            
                            channelEntry.Creator = creator;

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
                    .ToList()
                    .AsReadOnly();
            }
        }

        public async Task<bool> IsChannelExistsAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT 1
                                 FROM Channels
                                 WHERE Id = @channelId";
                return await connection.ExecuteScalarAsync<bool>(sqlQuery, new { channelId });
            }
        }

        public async Task<Channel> GetChannelAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM Channels
                                 WHERE Id = @channelId";

                return (await connection.QueryAsync<Channel>(sqlQuery, new { channelId })).FirstOrDefault();
            }
        }

        public async Task<Channel> GetChannelWithCreatorAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT c.Id, c.Created, c.Name, c.CreatorId, c.IsClosed, c.MembersCount, c.Type, c.Description, c.WelcomeMessage, c.Updated, c.PhotoUrl,
                                        m.Id, m.SaasUserId, m.Name, m.Status, m.Role, m.IsAfk, m.Email, m.LastActivity
                                 FROM Channels c 
                                 LEFT JOIN Members m ON c.CreatorId = m.Id
                                 WHERE c.Id = @channelId";

                return (await connection.QueryAsync<Channel, Member, Channel>(
                        sqlQuery,
                        (channel, member) =>
                        {
                            channel.Creator = member;
                            channel.CreatorId = member.Id;
                            return channel;
                        },
                        new { channelId }))
                    .FirstOrDefault();
            }
        }

        public async Task<bool> IsMemberExistsInChannelAsync(Guid memberId, Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT 1
                                 FROM ChannelMembers
                                 WHERE MemberId = @memberId AND ChannelId = @channelId";

                return (await connection.QueryAsync<bool>(sqlQuery, new { memberId, channelId })).FirstOrDefault();
            }
        }

        public async Task AddChannelAsync(Channel channel)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"INSERT INTO Channels(Id, Created, IsClosed, CreatorId, MembersCount, Updated, Name, Type, Description, WelcomeMessage, PhotoUrl) 
                                 VALUES (@Id, @Created, @IsClosed, @CreatorId, @MembersCount, @Updated, @Name, @Type, @Description, @WelcomeMessage, @PhotoUrl);";

                await connection.ExecuteScalarAsync(sqlQuery, channel);
            }
        }

        public async Task UpdateChannelAsync(Channel channel)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
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

        public async Task<IReadOnlyCollection<Channel>> GetAllowedChannelsAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM Channels c
                                 WHERE (c.Id IN
                                           (SELECT cm.ChannelId 
                                            FROM ChannelMembers cm 
                                            WHERE cm.MemberId = @memberId)) 
                                       AND c.IsClosed <> 1";

                return (await connection.QueryAsync<Channel>(sqlQuery, new { memberId })).ToList().AsReadOnly();
            }
        }

        public async Task IncrementChannelMembersCount(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
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
                var sqlQuery = @"UPDATE Channels
                                SET MembersCount = MembersCount - 1
                                WHERE Id = @channelId";

                await connection.ExecuteAsync(sqlQuery, new { channelId });
            }
        }
    }
}