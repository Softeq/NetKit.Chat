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
                var sqlQuery = $@"
                    SELECT
                        {nameof(Channel.Id)},
                        {nameof(Channel.IsClosed)},
                        {nameof(Channel.Created)},
                        {nameof(Channel.CreatorId)},
                        {nameof(Channel.Name)},
                        {nameof(Channel.MembersCount)},
                        {nameof(Channel.Type)},
                        {nameof(Channel.Description)},
                        {nameof(Channel.WelcomeMessage)},
                        {nameof(Channel.Updated)},
                        {nameof(Channel.PhotoUrl)}
                    FROM 
                        Channels";

                return (await connection.QueryAsync<Channel>(sqlQuery)).ToList().AsReadOnly();
            }
        }

        // TODO: Improve performance or split this method
        public async Task<IReadOnlyCollection<Channel>> GetAllowedChannelsWithMessagesAndCreatorAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
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
                        c.{nameof(Channel.PhotoUrl)},
                        creator.{nameof(Member.Id)}, 
                        creator.{nameof(Member.SaasUserId)}, 
                        creator.{nameof(Member.Name)}, 
                        creator.{nameof(Member.Status)}, 
                        creator.{nameof(Member.Role)}, 
                        creator.{nameof(Member.IsAfk)}, 
                        creator.{nameof(Member.Email)}, 
                        creator.{nameof(Member.LastActivity)},
                        m.{nameof(Message.Id)}, 
                        m.{nameof(Message.ChannelId)}, 
                        m.{nameof(Message.Created)}, 
                        m.{nameof(Message.Body)}, 
                        m.{nameof(Message.ImageUrl)}, 
                        m.{nameof(Message.Type)}, 
                        m.{nameof(Message.OwnerId)}, 
                        m.{nameof(Message.Updated)}, 
                        mem.{nameof(Member.Id)}, 
                        mem.{nameof(Member.SaasUserId)}, 
                        mem.{nameof(Member.Name)}, 
                        mem.{nameof(Member.Status)}, 
                        mem.{nameof(Member.Role)}, 
                        mem.{nameof(Member.IsAfk)}, 
                        mem.{nameof(Member.Email)}, 
                        mem.{nameof(Member.LastActivity)}
                    FROM 
                        Channels c
                    LEFT JOIN Members creator
                        ON c.{nameof(Channel.CreatorId)} = creator.{nameof(Member.Id)}
                    LEFT JOIN Messages m 
                        ON c.{nameof(Channel.Id)} = m.{nameof(Message.ChannelId)}
                    LEFT JOIN Members mem 
                        ON m.{nameof(Message.OwnerId)} = mem.{nameof(Member.Id)}
                    WHERE 
                        (c.{nameof(Channel.Id)} IN 
                            (SELECT 
                                cm.{nameof(ChannelMember.ChannelId)}
                            FROM 
                                ChannelMembers cm 
                            WHERE 
                                cm.{nameof(ChannelMember.MemberId)} = @{nameof(memberId)})) 
                        AND c.{nameof(Channel.IsClosed)} <> 1
                    ORDER BY 
                        m.{nameof(Message.Created)} DESC";

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
                var sqlQuery = $@"
                    SELECT 
                        1
                    FROM 
                        Channels
                    WHERE 
                        {nameof(Channel.Id)} = @{nameof(channelId)}";

                return await connection.ExecuteScalarAsync<bool>(sqlQuery, new { channelId });
            }
        }

        public async Task<Channel> GetChannelAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT
                        {nameof(Channel.Id)},
                        {nameof(Channel.IsClosed)},
                        {nameof(Channel.Created)},
                        {nameof(Channel.CreatorId)},
                        {nameof(Channel.Name)},
                        {nameof(Channel.MembersCount)},
                        {nameof(Channel.Type)},
                        {nameof(Channel.Description)},
                        {nameof(Channel.WelcomeMessage)},
                        {nameof(Channel.Updated)},
                        {nameof(Channel.PhotoUrl)}
                    FROM 
                        Channels
                    WHERE 
                        {nameof(Channel.Id)} = @{nameof(channelId)}";

                return (await connection.QueryAsync<Channel>(sqlQuery, new { channelId })).FirstOrDefault();
            }
        }

        public async Task<Channel> GetChannelWithCreatorAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
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
                        c.{nameof(Channel.PhotoUrl)},
                        m.{nameof(Member.Id)}, 
                        m.{nameof(Member.SaasUserId)}, 
                        m.{nameof(Member.Name)}, 
                        m.{nameof(Member.Status)}, 
                        m.{nameof(Member.Role)}, 
                        m.{nameof(Member.IsAfk)}, 
                        m.{nameof(Member.Email)}, 
                        m.{nameof(Member.LastActivity)}
                    FROM 
                        Channels c 
                    LEFT JOIN Members m 
                        ON c.{nameof(Channel.CreatorId)} = m.{nameof(Member.Id)}
                    WHERE 
                        c.{nameof(Channel.Id)} = @{nameof(channelId)}";

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
                var sqlQuery = $@"
                    SELECT 
                        1
                    FROM 
                        ChannelMembers
                    WHERE 
                        {nameof(ChannelMember.MemberId)} = @{nameof(memberId)}
                        AND {nameof(ChannelMember.ChannelId)} = @{nameof(channelId)}";

                return (await connection.QueryAsync<bool>(sqlQuery, new { memberId, channelId })).FirstOrDefault();
            }
        }

        public async Task AddChannelAsync(Channel channel)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    INSERT INTO Channels
                    (
                        {nameof(Channel.Id)}, 
                        {nameof(Channel.Created)}, 
                        {nameof(Channel.IsClosed)}, 
                        {nameof(Channel.CreatorId)}, 
                        {nameof(Channel.MembersCount)}, 
                        {nameof(Channel.Updated)}, 
                        {nameof(Channel.Name)}, 
                        {nameof(Channel.Type)}, 
                        {nameof(Channel.Description)}, 
                        {nameof(Channel.WelcomeMessage)}, 
                        {nameof(Channel.PhotoUrl)}
                    ) VALUES 
                    (
                        @{nameof(Channel.Id)}, 
                        @{nameof(Channel.Created)}, 
                        @{nameof(Channel.IsClosed)}, 
                        @{nameof(Channel.CreatorId)}, 
                        @{nameof(Channel.MembersCount)}, 
                        @{nameof(Channel.Updated)}, 
                        @{nameof(Channel.Name)}, 
                        @{nameof(Channel.Type)}, 
                        @{nameof(Channel.Description)}, 
                        @{nameof(Channel.WelcomeMessage)}, 
                        @{nameof(Channel.PhotoUrl)}
                    );";

                await connection.ExecuteScalarAsync(sqlQuery, channel);
            }
        }

        public async Task UpdateChannelAsync(Channel channel)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    UPDATE Channels 
                    SET 
                        {nameof(Channel.Name)} = @{nameof(Channel.Name)}, 
                        {nameof(Channel.Description)} = @{nameof(Channel.Description)}, 
                        {nameof(Channel.WelcomeMessage)} = @{nameof(Channel.WelcomeMessage)}, 
                        {nameof(Channel.MembersCount)} = @{nameof(Channel.MembersCount)},
                        {nameof(Channel.Type)} = @{nameof(Channel.Type)}, 
                        {nameof(Channel.IsClosed)} = @{nameof(Channel.IsClosed)}, 
                        {nameof(Channel.Updated)} = @{nameof(Channel.Updated)},
                        {nameof(Channel.PhotoUrl)} = @{nameof(Channel.PhotoUrl)}
                    WHERE
                        {nameof(Channel.Id)} = @{nameof(Channel.Id)}";

                await connection.ExecuteAsync(sqlQuery, channel);
            }
        }

        public async Task<IReadOnlyCollection<Channel>> GetAllowedChannelsAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        {nameof(Channel.Id)},
                        {nameof(Channel.IsClosed)},
                        {nameof(Channel.Created)},
                        {nameof(Channel.CreatorId)},
                        {nameof(Channel.Name)},
                        {nameof(Channel.MembersCount)},
                        {nameof(Channel.Type)},
                        {nameof(Channel.Description)},
                        {nameof(Channel.WelcomeMessage)},
                        {nameof(Channel.Updated)},
                        {nameof(Channel.PhotoUrl)}
                    FROM
                        Channels c
                    WHERE
                        (c.{nameof(Channel.Id)} IN
                            (SELECT 
                                cm.{nameof(ChannelMember.ChannelId)}
                            FROM 
                                ChannelMembers cm 
                            WHERE 
                                cm.{nameof(ChannelMember.MemberId)} = @{nameof(memberId)})) 
                    AND c.{nameof(Channel.IsClosed)} <> 1";

                return (await connection.QueryAsync<Channel>(sqlQuery, new { memberId })).ToList().AsReadOnly();
            }
        }

        public async Task IncrementChannelMembersCount(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    UPDATE Channels
                    SET 
                        {nameof(Channel.MembersCount)} = {nameof(Channel.MembersCount)} + 1
                    WHERE 
                        {nameof(Channel.Id)} = @{nameof(channelId)}";

                await connection.ExecuteAsync(sqlQuery, new { channelId });
            }
        }

        public async Task DecrementChannelMembersCount(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    UPDATE Channels
                    SET 
                        {nameof(Channel.MembersCount)} = {nameof(Channel.MembersCount)} - 1
                    WHERE 
                        {nameof(Channel.Id)} = @{nameof(channelId)}";

                await connection.ExecuteAsync(sqlQuery, new { channelId });
            }
        }

        public async Task<Guid> GetDirectChannelForMembersAsync(Guid member1Id, Guid member2Id)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        c.{nameof(Channel.Id)}
                    FROM 
                        Channels c
                    INNER JOIN ChannelMembers cm 
                        ON c.{nameof(Channel.Id)} = cm.{nameof(ChannelMember.ChannelId)}
                    WHERE
                        (cm.{nameof(ChannelMember.MemberId)} = @{nameof(member1Id)} OR cm.{nameof(ChannelMember.MemberId)} = @{nameof(member2Id)}) 
                        AND c.{nameof(Channel.Type)} = {(int)ChannelType.Direct}
                    GROUP BY 
                        c.{nameof(Channel.Id)}
                    HAVING 
                        COUNT(cm.{nameof(ChannelMember.MemberId)}) = 2";

                return (await connection.QueryAsync<Guid>(sqlQuery, new { member1Id, member2Id })).FirstOrDefault();
            }
        }
    }
}