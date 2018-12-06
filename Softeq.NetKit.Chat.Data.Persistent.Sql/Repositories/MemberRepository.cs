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
                var whereCondition = !string.IsNullOrEmpty(nameFilter) ? " WHERE LOWER(Name) LIKE LOWER('%' + @nameFilter + '%')" : "";

                var sqlQuery = @"SELECT *
                                 FROM Members " +
                                 whereCondition +
                               @" ORDER BY Name
                                 OFFSET @pageSize * (@pageNumber - 1) ROWS
                                 FETCH NEXT @pageSize ROWS ONLY

                                 SELECT COUNT(*)
                                 FROM Members" +
                                 whereCondition;

                var data = await connection.QueryMultipleAsync(sqlQuery, new { pageNumber, pageSize, nameFilter });

                var members = await data.ReadAsync<Member>();
                var totalRows = await data.ReadSingleAsync<int>();

                return new QueryResult<Member>
                {
                    Entities = members,
                    TotalRows = totalRows,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }

        public async Task<QueryResult<Member>> GetPotentialChannelMembersAsync(Guid channelId, int pageNumber, int pageSize, string nameFilter)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var whereCondition = @"WHERE Id NOT IN (
                                        SELECT ChannelMembers.MemberId
                                        FROM Members
                                        INNER JOIN ChannelMembers
                                        ON Members.Id = ChannelMembers.MemberId
                                        WHERE ChannelMembers.ChannelId = @channelId)" +
                                        (!string.IsNullOrEmpty(nameFilter) ? " AND LOWER(Members.Name) LIKE LOWER('%' + @nameFilter + '%')" : "");

                var sqlQuery = @"SELECT *  
                                 FROM Members " +
                                 whereCondition +
                               @" ORDER BY Name
                                 OFFSET @pageSize * (@pageNumber - 1) ROWS
                                 FETCH NEXT @pageSize ROWS ONLY

                                 SELECT COUNT(*) 
                                 FROM Members " +
                                 whereCondition;

                var data = await connection.QueryMultipleAsync(sqlQuery, new { channelId, pageNumber, pageSize, nameFilter });

                var members = await data.ReadAsync<Member>();
                var totalRows = await data.ReadSingleAsync<int>();

                return new QueryResult<Member>
                {
                    Entities = members,
                    TotalRows = totalRows,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        }

        public async Task<Member> GetMemberByIdAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM Members
                                 WHERE Id = @memberId";

                return (await connection.QueryAsync<Member>(sqlQuery, new { memberId })).FirstOrDefault();
            }
        }

        public async Task AddMemberAsync(Member member)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"INSERT INTO Members(Id, Email, IsAfk, IsBanned, LastActivity, LastNudged, Name, PhotoName, Role, SaasUserId, Status) 
                                 VALUES (@Id, @Email, @IsAfk, @IsBanned, @LastActivity, @LastNudged, @Name, @PhotoName, @Role, @SaasUserId, @Status)";

                await connection.ExecuteScalarAsync(sqlQuery, member);
            }
        }

        public async Task UpdateMemberAsync(Member member)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"UPDATE Members 
                                 SET Email = @Email, 
                                     IsAfk = @IsAfk, 
                                     IsBanned = @IsBanned, 
                                     LastActivity = @LastActivity, 
                                     LastNudged = @LastNudged, 
                                     Name = @Name, 
                                     PhotoName = @PhotoName,
                                     Role = @Role, 
                                     SaasUserId = @SaasUserId, 
                                     Status = @Status
                                 WHERE Id = @Id";

                await connection.ExecuteScalarAsync(sqlQuery, member);
            }
        }

        public async Task<Member> GetMemberBySaasUserIdAsync(string saasUserId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT * 
                                 FROM Members
                                 WHERE SaasUserId = @saasUserId";

                return (await connection.QueryAsync<Member>(sqlQuery, new { saasUserId })).FirstOrDefault();
            }
        }

        public async Task<IReadOnlyCollection<Member>> GetAllMembersByChannelIdAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT m.Id, m.Email, m.IsAfk, m.IsBanned, m.LastActivity, m.LastNudged, m.Name, m.PhotoName, m.Role, m.SaasUserId, m.Status  
                                 FROM Members m
                                 INNER JOIN ChannelMembers c ON m.Id = c.MemberId
                                 WHERE c.ChannelId = @channelId";

                return (await connection.QueryAsync<Member>(sqlQuery, new { channelId })).Distinct().ToList().AsReadOnly();
            }
        }
    }
}