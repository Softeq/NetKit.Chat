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
    internal class MemberRepository : IMemberRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public MemberRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<QueryResult<Member>> GetPagedMembersAsync(int pageNumber, int pageSize, string nameFilter)
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
                        {nameof(Member.IsActive)}
                    FROM 
                        Members 
                    {(!string.IsNullOrEmpty(nameFilter) ? $" WHERE LOWER({nameof(Member.Name)}) LIKE LOWER('%' + @{nameof(nameFilter)} + '%')" : "")} 
                    ORDER BY 
                        {nameof(Member.Name)}
                    OFFSET @{nameof(pageSize)} * (@{nameof(pageNumber)} - 1) ROWS
                    FETCH NEXT @{nameof(pageSize)} ROWS ONLY
                    
                    SELECT 
                        COUNT(*)
                    FROM 
                        Members 
                    {(!string.IsNullOrEmpty(nameFilter) ? $" WHERE LOWER({nameof(Member.Name)}) LIKE LOWER('%' + @{nameof(nameFilter)} + '%')" : "")}";

                var data = await connection.QueryMultipleAsync(sqlQuery, new { pageNumber, pageSize, nameFilter });

                var members = await data.ReadAsync<Member>();
                var totalRows = await data.ReadSingleAsync<int>();

                return new QueryResult<Member>
                {
                    Results = members,
                    TotalNumberOfItems = totalRows,
                    TotalNumberOfPages = (totalRows / pageSize) + ((totalRows % pageSize) == 0 ? 0 : 1),
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }

        public async Task<QueryResult<Member>> GetPotentialChannelMembersAsync(Guid channelId, int pageNumber, int pageSize, string nameFilter)
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
                        {nameof(Member.IsActive)}
                    FROM 
                        Members 
                    WHERE 
                        {nameof(Member.Id)} NOT IN (
                            SELECT 
                                ChannelMembers.{nameof(ChannelMember.MemberId)}
                            FROM 
                                Members
                            INNER JOIN ChannelMembers
                                ON Members.{nameof(Member.Id)} = ChannelMembers.{nameof(ChannelMember.MemberId)}
                            WHERE 
                                ChannelMembers.{nameof(ChannelMember.ChannelId)} = @{nameof(channelId)})
                                {(!string.IsNullOrEmpty(nameFilter) ? $" AND LOWER(Members.{nameof(Member.Name)}) LIKE LOWER('%' + @{nameof(nameFilter)} + '%')" : "")}
                    ORDER BY 
                        {nameof(Member.Name)}
                    OFFSET @{nameof(pageSize)} * (@{nameof(pageNumber)} - 1) ROWS
                    FETCH NEXT @{nameof(pageSize)} ROWS ONLY

                    SELECT 
                        COUNT(*) 
                    FROM 
                        Members 
                    WHERE 
                        {nameof(Member.Id)} NOT IN (
                            SELECT 
                                ChannelMembers.{nameof(ChannelMember.MemberId)}
                            FROM 
                                Members
                            INNER JOIN ChannelMembers
                                ON Members.{nameof(Member.Id)} = ChannelMembers.{nameof(ChannelMember.MemberId)}
                            WHERE 
                                ChannelMembers.{nameof(ChannelMember.ChannelId)} = @{nameof(channelId)})
                                {(!string.IsNullOrEmpty(nameFilter) ? $" AND LOWER(Members.{nameof(Member.Name)}) LIKE LOWER('%' + @{nameof(nameFilter)} + '%')" : "")}";

                var data = await connection.QueryMultipleAsync(sqlQuery, new { channelId, pageNumber, pageSize, nameFilter });

                var members = await data.ReadAsync<Member>();
                var totalRows = await data.ReadSingleAsync<int>();

                return new QueryResult<Member>
                {
                    Results = members,
                    TotalNumberOfItems = totalRows,
                    TotalNumberOfPages = (totalRows / pageSize) + ((totalRows % pageSize) == 0 ? 0 : 1),
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }

        public async Task<Member> GetMemberByIdAsync(Guid memberId)
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
                        {nameof(Member.IsActive)}
                    FROM 
                        Members
                    WHERE 
                        {nameof(Member.Id)} = @{nameof(memberId)}";

                return (await connection.QueryAsync<Member>(sqlQuery, new { memberId })).FirstOrDefault();
            }
        }

        public async Task AddMemberAsync(Member member)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    INSERT INTO Members
                    (
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
                        {nameof(Member.Status)}
                    ) VALUES (
                        @{nameof(Member.Id)}, 
                        @{nameof(Member.Email)}, 
                        @{nameof(Member.IsAfk)}, 
                        @{nameof(Member.IsBanned)}, 
                        @{nameof(Member.LastActivity)}, 
                        @{nameof(Member.LastNudged)}, 
                        @{nameof(Member.Name)}, 
                        @{nameof(Member.PhotoName)}, 
                        @{nameof(Member.Role)}, 
                        @{nameof(Member.SaasUserId)}, 
                        @{nameof(Member.Status)}
                    )";

                await connection.ExecuteScalarAsync(sqlQuery, member);
            }
        }

        public async Task UpdateMemberAsync(Member member)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    UPDATE Members 
                    SET 
                        {nameof(Member.Email)} = @{nameof(Member.Email)},
                        {nameof(Member.IsAfk)} = @{nameof(Member.IsAfk)}, 
                        {nameof(Member.IsBanned)} = @{nameof(Member.IsBanned)}, 
                        {nameof(Member.LastActivity)} = @{nameof(Member.LastActivity)}, 
                        {nameof(Member.LastNudged)} = @{nameof(Member.LastNudged)}, 
                        {nameof(Member.Name)} = @{nameof(Member.Name)}, 
                        {nameof(Member.PhotoName)} = @{nameof(Member.PhotoName)},
                        {nameof(Member.Role)} = @{nameof(Member.Role)}, 
                        {nameof(Member.SaasUserId)} = @{nameof(Member.SaasUserId)}, 
                        {nameof(Member.Status)} = @{nameof(Member.Status)}
                    WHERE 
                        {nameof(Member.Id)} = @{nameof(Member.Id)}";

                await connection.ExecuteScalarAsync(sqlQuery, member);
            }
        }

        public async Task ActivateMemberAsync(Member member)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    UPDATE Members 
                    SET 
                        {nameof(Member.IsActive)} = @{nameof(Member.IsActive)}
                    WHERE 
                        {nameof(Member.Id)} = @{nameof(Member.Id)}";

                await connection.ExecuteScalarAsync(sqlQuery, member);
            }
        }

        public async Task<Member> GetMemberBySaasUserIdAsync(string saasUserId)
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
                        {nameof(Member.IsActive)}
                    FROM 
                        Members
                    WHERE 
                        {nameof(Member.SaasUserId)} = @{nameof(saasUserId)}";

                return (await connection.QueryAsync<Member>(sqlQuery, new { saasUserId })).FirstOrDefault();
            }
        }

        public async Task<IReadOnlyCollection<Member>> GetAllMembersByChannelIdAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        m.{nameof(Member.Id)}, 
                        m.{nameof(Member.Email)}, 
                        m.{nameof(Member.IsAfk)}, 
                        m.{nameof(Member.IsBanned)}, 
                        m.{nameof(Member.LastActivity)}, 
                        m.{nameof(Member.LastNudged)}, 
                        m.{nameof(Member.Name)}, 
                        m.{nameof(Member.PhotoName)}, 
                        m.{nameof(Member.Role)}, 
                        m.{nameof(Member.SaasUserId)}, 
                        m.{nameof(Member.Status)}
                    FROM 
                        Members m
                    INNER JOIN ChannelMembers c 
                        ON m.{nameof(Member.Id)} = c.{nameof(ChannelMember.MemberId)}
                    WHERE 
                        c.{nameof(ChannelMember.ChannelId)} = @{nameof(channelId)}";

                return (await connection.QueryAsync<Member>(sqlQuery, new { channelId })).Distinct().ToList().AsReadOnly();
            }
        }
    }
}