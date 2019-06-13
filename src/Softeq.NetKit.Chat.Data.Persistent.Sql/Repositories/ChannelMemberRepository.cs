// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Repositories
{
    internal class ChannelMemberRepository : BaseRepository, IChannelMemberRepository
    {
        public ChannelMemberRepository(ISqlConnectionFactory sqlConnectionFactory) : base(sqlConnectionFactory)
        {
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
                        {nameof(ChannelMember.Role)},
                        {nameof(ChannelMember.IsMuted)}, 
                        {nameof(ChannelMember.IsPinned)}
                    ) VALUES 
                    (
                        @{nameof(ChannelMember.ChannelId)}, 
                        @{nameof(ChannelMember.MemberId)}, 
                        @{nameof(ChannelMember.LastReadMessageId)}, 
                        @{nameof(ChannelMember.Role)},
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
                        {nameof(ChannelMember.Role)},
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

        public async Task<ChannelMember> GetChannelMemberWithMemberDetailsAsync(Guid memberId, Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"SELECT                         
                                    cm.{nameof(ChannelMember.ChannelId)}, 
                                    cm.{nameof(ChannelMember.MemberId)}, 
                                    cm.{nameof(ChannelMember.LastReadMessageId)}, 
                                    cm.{nameof(ChannelMember.Role)}, 
                                    cm.{nameof(ChannelMember.IsMuted)}, 
                                    cm.{nameof(ChannelMember.IsPinned)},
                                    m.{nameof(Member.Email)},
                                    m.{nameof(Member.Id)},                                    
                                    m.{nameof(Member.IsBanned)},
                                    m.{nameof(Member.LastActivity)},
                                    m.{nameof(Member.LastNudged)},
                                    m.{nameof(Member.Name)},
                                    m.{nameof(Member.PhotoName)},
                                    m.{nameof(Member.SaasUserId)},
                                    m.{nameof(Member.Status)},
                                    m.{nameof(Member.IsActive)},
                                    m.{nameof(Member.IsDeleted)}
                                 FROM 
                                    ChannelMembers cm
                                 INNER JOIN 
                                    Members m ON m.{nameof(Member.Id)} = cm.{nameof(ChannelMember.MemberId)}
                                 WHERE 
                                    cm.{nameof(ChannelMember.ChannelId)} = @{nameof(channelId)} AND cm.{nameof(ChannelMember.MemberId)} = @{nameof(memberId)}";

                return (await connection.QueryAsync<ChannelMember, Member, ChannelMember>(sqlQuery, (channelMember, member) =>
                {
                    channelMember.Member = member;
                    channelMember.MemberId = member.Id;
                    return channelMember;
                }, new { memberId, channelId })).FirstOrDefault();
            }
        }

        public async Task<ChannelMemberAggregate> GetChannelMemberWithLastReadMessageAndCounterAsync(Guid channelId, Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        cm.{nameof(ChannelMember.ChannelId)}, 
                        cm.{nameof(ChannelMember.MemberId)}, 
                        cm.{nameof(ChannelMember.LastReadMessageId)}, 
                        cm.{nameof(ChannelMember.Role)}, 
                        cm.{nameof(ChannelMember.IsMuted)}, 
                        cm.{nameof(ChannelMember.IsPinned)},
                        mm.{nameof(Member.Id)}, 
                        mm.{nameof(Member.Email)},       
                        mm.{nameof(Member.IsBanned)},
                        mm.{nameof(Member.LastActivity)},
                        mm.{nameof(Member.LastNudged)},
                        mm.{nameof(Member.Name)},
                        mm.{nameof(Member.PhotoName)},
                        mm.{nameof(Member.SaasUserId)},
                        mm.{nameof(Member.Status)},
                        mm.{nameof(Member.IsActive)},
                        mm.{nameof(Member.IsDeleted)},
                        m.{nameof(Message.Id)}, 
                        m.{nameof(Message.ChannelId)}, 
                        m.{nameof(Message.Created)}, 
                        m.{nameof(Message.Body)}, 
                        m.{nameof(Message.ImageUrl)}, 
                        m.{nameof(Message.Type)}, 
                        m.{nameof(Message.OwnerId)}, 
                        m.{nameof(Message.Updated)}, 
                        m.{nameof(Message.AccessibilityStatus)},
                        (SELECT COUNT(*)
                         FROM Messages m
                         WHERE m.{nameof(Message.ChannelId)} = @{nameof(channelId)} AND m.{nameof(Message.AccessibilityStatus)} = @{nameof(Message.AccessibilityStatus)} AND m.{nameof(Message.Created)} < (
                             SELECT m.{nameof(Message.Created)}
	                         FROM ChannelMembers cm
	                         LEFT JOIN Messages m
	                         ON m.{nameof(Message.Id)} = cm.{nameof(ChannelMember.LastReadMessageId)}
	                         WHERE cm.{nameof(ChannelMember.MemberId)} = @{nameof(memberId)} AND cm.{nameof(ChannelMember.ChannelId)} = @{nameof(channelId)} AND m.{nameof(Message.AccessibilityStatus)} = @{nameof(Message.AccessibilityStatus)})) as {nameof(ChannelMemberAggregate.UnreadMessagesCount)}
                   FROM ChannelMembers cm
                   INNER JOIN Members mm
                   ON mm.{nameof(Member.Id)} = cm.{nameof(ChannelMember.MemberId)}
                   LEFT JOIN Messages m
                   ON m.{nameof(Message.Id)} = cm.{nameof(ChannelMember.LastReadMessageId)}
                   WHERE cm.{nameof(ChannelMember.MemberId)} = @{nameof(memberId)} AND cm.{nameof(ChannelMember.ChannelId)} = @{nameof(channelId)}";

                return (await connection.QueryAsync<ChannelMember, Member, Message, int, ChannelMemberAggregate>(sqlQuery,
                    (channelMember, member, message, unreadCount) =>
                    {
                        channelMember.Member = member;

                        var cmAggregate = new ChannelMemberAggregate
                        {
                            ChannelMember = channelMember,
                            Message = message,
                            UnreadMessagesCount = unreadCount
                        };

                        return cmAggregate;

                    }, new { channelId, memberId, AccessibilityStatus = AccessibilityStatus.Present }, null, true, "Id, Id, UnreadMessagesCount"
                )).FirstOrDefault();
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
                        {nameof(ChannelMember.Role)}, 
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
                        cm.{nameof(ChannelMember.ChannelId)}, 
                        cm.{nameof(ChannelMember.MemberId)}, 
                        cm.{nameof(ChannelMember.LastReadMessageId)}, 
                        cm.{nameof(ChannelMember.Role)}, 
                        cm.{nameof(ChannelMember.IsMuted)}, 
                        cm.{nameof(ChannelMember.IsPinned)},
                        m.{nameof(Member.Email)},
                        m.{nameof(Member.Id)},                        
                        m.{nameof(Member.IsBanned)},
                        m.{nameof(Member.LastActivity)},
                        m.{nameof(Member.LastNudged)},
                        m.{nameof(Member.Name)},
                        m.{nameof(Member.PhotoName)},
                        m.{nameof(Member.SaasUserId)},
                        m.{nameof(Member.Status)},
                        m.{nameof(Member.IsActive)},
                        m.{nameof(Member.IsDeleted)}
                    FROM 
                        ChannelMembers cm
                    INNER JOIN Members m 
                        ON cm.{nameof(ChannelMember.MemberId)} = m.{nameof(Member.Id)}
                    WHERE cm.{nameof(ChannelMember.ChannelId)} = @{nameof(channelId)}";

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