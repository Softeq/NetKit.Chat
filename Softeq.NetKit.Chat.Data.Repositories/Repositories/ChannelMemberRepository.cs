// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Softeq.NetKit.Chat.Data.Interfaces.Repository;
using Softeq.NetKit.Chat.Data.Repositories.Infrastructure;
using Softeq.NetKit.Chat.Domain.ChannelMember;

namespace Softeq.NetKit.Chat.Data.Repositories.Repositories
{
    internal class ChannelMemberRepository : IChannelMemberRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public ChannelMemberRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task AddChannelMemberAsync(ChannelMembers channelMember)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    INSERT INTO ChannelMembers(ChannelId, MemberId, LastReadMessageId, IsMuted, IsPinned) 
                    VALUES (@ChannelId, @MemberId, @LastReadMessageId, @IsMuted, @IsPinned)";

                await connection.ExecuteScalarAsync(sqlQuery, channelMember);
            }
        }

        public async Task DeleteChannelMemberAsync(Guid memberId, Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                DELETE FROM ChannelMembers WHERE ChannelId = @channelId AND MemberId = @memberId";

                await connection.ExecuteAsync(sqlQuery, new { channelId, memberId });
            }
        }

        public async Task UpdateChannelMemberAsync(ChannelMembers channelMember)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"UPDATE ChannelMembers 
                                 SET ChannelId = @ChannelId, 
                                     MemberId = @MemberId, 
                                     LastReadMessageId = @LastReadMessageId, 
                                     IsMuted = @IsMuted,
                                     IsPinned = @IsPinned
                                 WHERE ChannelId = @ChannelId AND MemberId = @MemberId";
                
                await connection.ExecuteAsync(sqlQuery, channelMember);
            }
        }

        public async Task<List<ChannelMembers>> GetChannelMembersAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT MemberId, ChannelId, LastReadMessageId, IsMuted, IsPinned
                    FROM ChannelMembers
                    WHERE ChannelId = @channelId";

                var data = (await connection.QueryAsync<ChannelMembers>(sqlQuery, new { channelId })).ToList();

                return data;
            }
        }

        public async Task MuteChannelAsync(Guid memberId, Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"UPDATE ChannelMembers
                                SET IsMuted = 1
                                WHERE ChannelId = @channelId AND MemberId = @memberId";

                await connection.ExecuteAsync(sqlQuery, new { channelId, memberId });
            }
        }

        public async Task PinChannelAsync(Guid memberId, Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"UPDATE ChannelMembers
                                SET IsPinned = IsPinned^1
                                WHERE ChannelId = @channelId AND MemberId = @memberId";

                await connection.ExecuteAsync(sqlQuery, new { channelId, memberId });
            }
        }

        public async Task AddLastReadMessageAsync(Guid memberId, Guid channelId, Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"UPDATE ChannelMembers
                                SET LastReadMessageId = @messageId
                                WHERE ChannelId = @channelId AND MemberId = @memberId";

                await connection.ExecuteAsync(sqlQuery, new { channelId, memberId, messageId });
            }
        }

        public async Task UpdateLastReadMessageAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"UPDATE ChannelMembers
                                SET LastReadMessageId = NULL
                                WHERE LastReadMessageId = @messageId";

                await connection.ExecuteAsync(sqlQuery, new { messageId });
            }
        }

        public async Task<ChannelMembers> GetChannelMemberAsync(Guid memberId, Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT ChannelId, MemberId, LastReadMessageId, IsMuted, IsPinned 
                    FROM ChannelMembers
                    WHERE ChannelId = @channelId AND MemberId = @memberId";

                var data = (await connection.QueryAsync<ChannelMembers>(sqlQuery, new { memberId, channelId }))
                    .FirstOrDefault();

                return data;
            }
        }
    }
}