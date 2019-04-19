﻿// Developed by Softeq Development Corporation
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
    internal class ChannelMemberRepository : IChannelMemberRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public ChannelMemberRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task AddChannelMemberAsync(ChannelMember channelMember)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    INSERT INTO ChannelMembers
                    (
                        {nameof(ChannelMember.ChannelId)}, 
                        {nameof(ChannelMember.MemberId)}, 
                        {nameof(ChannelMember.LastReadMessageId)}, 
                        {nameof(ChannelMember.IsMuted)}, 
                        {nameof(ChannelMember.IsPinned)}
                    ) VALUES 
                    (
                        @{nameof(ChannelMember.ChannelId)}, 
                        @{nameof(ChannelMember.MemberId)}, 
                        @{nameof(ChannelMember.LastReadMessageId)}, 
                        @{nameof(ChannelMember.IsMuted)}, 
                        @{nameof(ChannelMember.IsPinned)}
                    )";

                await connection.ExecuteScalarAsync(sqlQuery, channelMember);
            }
        }

        public async Task<ChannelMember> GetChannelMemberAsync(Guid memberId, Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT
                        {nameof(ChannelMember.ChannelId)}, 
                        {nameof(ChannelMember.MemberId)}, 
                        {nameof(ChannelMember.LastReadMessageId)}, 
                        {nameof(ChannelMember.IsMuted)}, 
                        {nameof(ChannelMember.IsPinned)}
                    FROM 
                        ChannelMembers
                    WHERE 
                        {nameof(ChannelMember.ChannelId)} = @{nameof(channelId)} 
                        AND {nameof(ChannelMember.MemberId)} = @{nameof(memberId)}";

                return (await connection.QueryAsync<ChannelMember>(sqlQuery, new { memberId, channelId })).FirstOrDefault();
            }
        }

        public async Task DeleteChannelMemberAsync(Guid memberId, Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    DELETE FROM ChannelMembers 
                    WHERE 
                        {nameof(ChannelMember.ChannelId)} = @{nameof(channelId)} 
                        AND {nameof(ChannelMember.MemberId)} = @{nameof(memberId)}";

                await connection.ExecuteAsync(sqlQuery, new { channelId, memberId });
            }
        }

        public async Task<IReadOnlyCollection<ChannelMember>> GetChannelMembersAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT
                        {nameof(ChannelMember.ChannelId)}, 
                        {nameof(ChannelMember.MemberId)}, 
                        {nameof(ChannelMember.LastReadMessageId)}, 
                        {nameof(ChannelMember.IsMuted)}, 
                        {nameof(ChannelMember.IsPinned)}
                    FROM 
                        ChannelMembers
                    WHERE
                        {nameof(ChannelMember.ChannelId)} = @{nameof(channelId)}";

                return (await connection.QueryAsync<ChannelMember>(sqlQuery, new { channelId })).ToList().AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<ChannelMember>> GetChannelMembersWithMemberDetailsAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        {nameof(Member.Id)},
                        {nameof(Member.Email)},
                        {nameof(Member.IsAfk)},
                        {nameof(Member.IsBanned)},
                        {nameof(Member.LastActivity)},
                        {nameof(Member.LastNudged)},
                        {nameof(Member.Name)},
                        {nameof(Member.PhotoName)},
                        {nameof(Member.Role)},
                        {nameof(Member.SaasUserId)},
                        {nameof(Member.Status)},
                        {nameof(Member.IsActive)},
                        {nameof(Member.IsDeleted)},
                        {nameof(ChannelMember.ChannelId)}, 
                        {nameof(ChannelMember.MemberId)}, 
                        {nameof(ChannelMember.LastReadMessageId)}, 
                        {nameof(ChannelMember.IsMuted)}, 
                        {nameof(ChannelMember.IsPinned)}
                    FROM 
                        ChannelMembers cm
                    INNER JOIN Members m 
                        ON m.{nameof(Member.Id)} = cm.{nameof(ChannelMember.MemberId)}
                    WHERE {nameof(ChannelMember.ChannelId)} = @{nameof(channelId)}";

                return (await connection.QueryAsync<ChannelMember, Member, ChannelMember>(
                    sqlQuery,
                    (channelMember, member) =>
                    {
                        channelMember.Member = member;
                        channelMember.MemberId = member.Id;
                        return channelMember;
                    },
                    new {channelId})).ToList().AsReadOnly();
            }
        }

        public async Task MuteChannelAsync(Guid memberId, Guid channelId, bool isMuted)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    UPDATE ChannelMembers
                    SET 
                        {nameof(ChannelMember.IsMuted)} = @{nameof(isMuted)}
                    WHERE 
                        {nameof(ChannelMember.ChannelId)} = @{nameof(channelId)}
                        AND {nameof(ChannelMember.MemberId)} = @{nameof(memberId)}";

                await connection.ExecuteAsync(sqlQuery, new { channelId, memberId, isMuted });
            }
        }

        public async Task PinChannelAsync(Guid memberId, Guid channelId, bool isPinned)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    UPDATE ChannelMembers
                    SET
                         {nameof(ChannelMember.IsPinned)} = @{nameof(isPinned)}
                    WHERE 
                        {nameof(ChannelMember.ChannelId)} = @{nameof(channelId)}
                        AND {nameof(ChannelMember.MemberId)} = @{nameof(memberId)}";

                await connection.ExecuteAsync(sqlQuery, new { isPinned, channelId, memberId });
            }
        }

        public async Task SetLastReadMessageAsync(Guid memberId, Guid channelId, Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    UPDATE ChannelMembers
                    SET 
                        {nameof(ChannelMember.LastReadMessageId)} = @{nameof(messageId)}
                    WHERE
                        {nameof(ChannelMember.ChannelId)} = @{nameof(channelId)}
                        AND {nameof(ChannelMember.MemberId)} = @{nameof(memberId)}";

                await connection.ExecuteAsync(sqlQuery, new { channelId, memberId, messageId });
            }
        }

        public async Task UpdateLastReadMessageAsync(Guid previousLastReadMessageId, Guid? currentLastReadMessageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    UPDATE ChannelMembers
                    SET 
                        {nameof(ChannelMember.LastReadMessageId)} = @{nameof(currentLastReadMessageId)}
                    WHERE 
                        {nameof(ChannelMember.LastReadMessageId)} = @{nameof(previousLastReadMessageId)}";

                await connection.ExecuteAsync(sqlQuery, new { previousLastReadMessageId, currentLastReadMessageId });
            }
        }

        public async Task<IList<string>> GetSaasUserIdsWithDisabledChannelNotificationsAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        {nameof(Member.SaasUserId)}
                    FROM 
                        ChannelMembers
                    INNER JOIN Members
                        ON Members.{nameof(Member.Id)} = ChannelMembers.{nameof(ChannelMember.MemberId)}
                    WHERE 
                        {nameof(ChannelMember.ChannelId)} = @{nameof(channelId)}
                        AND {nameof(ChannelMember.IsMuted)} = 1";

                return (await connection.QueryAsync<string>(sqlQuery, new { channelId })).ToList().AsReadOnly();
            }
        }
    }
}