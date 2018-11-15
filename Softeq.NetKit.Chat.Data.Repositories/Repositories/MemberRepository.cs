﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Softeq.NetKit.Chat.Data.Interfaces.Repository;
using Softeq.NetKit.Chat.Data.Repositories.Infrastructure;
using Softeq.NetKit.Chat.Domain.Member;

namespace Softeq.NetKit.Chat.Data.Repositories.Repositories
{
    internal class MemberRepository : IMemberRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public MemberRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<List<Member>> GetAllMembersAsync()
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, Email, IsAfk, IsBanned, LastActivity, LastNudged, Name, PhotoName, Role, SaasUserId, Status  
                    FROM Members";

                var data = (await connection.QueryAsync<Member>(sqlQuery)).ToList();

                return data;
            }
        }

        public async Task<List<Member>> GetOnlineMembersInChannelAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT m.Id, m.Email, m.IsAfk, m.IsBanned, m.LastActivity, m.LastNudged, m.Name, m.PhotoName, m.Role, m.SaasUserId, m.Status  
                    FROM Members m
                    INNER JOIN ChannelMembers c ON m.Id = c.MemberId
                    WHERE m.Status != 2 AND c.ChannelId = @channelId";
                
                var data = (await connection.QueryAsync<Member>(sqlQuery, new { channelId })).ToList();

                return data;
            }
        }

        public async Task<List<Member>> GetAllOnlineMembersAsync()
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, Email, IsAfk, IsBanned, LastActivity, LastNudged, Name, PhotoName, Role, SaasUserId, Status  
                    FROM Members
                    WHERE Status != 2";

                var data = (await connection.QueryAsync<Member>(sqlQuery)).ToList();

                return data;
            }
        }

        public async Task<List<Member>> SearchMembersByNameAsync(string name)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, Email, IsAfk, IsBanned, LastActivity, LastNudged, Name, PhotoName, Role, SaasUserId, Status 
                    FROM Members
                    WHERE Name LIKE @name";
                
                var data = (await connection.QueryAsync<Member>(sqlQuery, new { name })).ToList();
                
                return data;
            }
        }

        public async Task<Member> GetMemberByIdAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, Email, IsAfk, IsBanned, LastActivity, LastNudged, Name, PhotoName, Role, SaasUserId, Status 
                    FROM Members
                    WHERE Id = @memberId";

                var data = (await connection.QueryAsync<Member>(sqlQuery, new { memberId }))
                    .FirstOrDefault();
                
                return data;
            }
        }

        public async Task<Member> GetMemberByNameAsync(string userName)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, Email, IsAfk, IsBanned, LastActivity, LastNudged, Name, PhotoName, Role, SaasUserId, Status  
                    FROM Members
                    WHERE Name = @userName";
                
                var data = (await connection.QueryAsync<Member>(sqlQuery, new { userName }))
                    .FirstOrDefault();
                
                return data;
            }
        }

        public async Task<Member> GetMemberByClientIdAsync(Guid clientId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT m.Id, m.Email, m.IsAfk, m.IsBanned, m.LastActivity, m.LastNudged, m.Name, m.PhotoName, m.Role, m.SaasUserId, m.Status 
                    FROM Members m
                    INNER JOIN Connections c ON m.Id = c.MemberId
                    WHERE c.Id = @clientId";
                
                var data = (await connection.QueryAsync<Member>(sqlQuery, new { clientId }))
                    .FirstOrDefault();

                return data;
            }
        }

        public async Task AddMemberAsync(Member member)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    INSERT INTO Members(Id, Email, IsAfk, IsBanned, LastActivity, LastNudged, Name, Role, SaasUserId, Status) 
                    VALUES (@Id, @Email, @IsAfk, @IsBanned, @LastActivity, @LastNudged, @Name, @Role, @SaasUserId, @Status);";
                
                await connection.ExecuteScalarAsync(sqlQuery, member);
            }
        }

        //TODO: Add Unit Test
        public async Task UpdateMemberAsync(Member member)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"UPDATE Members 
                                 SET Id = @Id, 
                                     Email = @Email, 
                                     IsAfk = @IsAfk, 
                                     IsBanned = @IsBanned, 
                                     LastActivity = @LastActivity, 
                                     LastNudged = @LastNudged, 
                                     Name = @Name, 
                                     Role = @Role, 
                                     SaasUserId = @SaasUserId, 
                                     Status = @Status
                                 WHERE Id = @Id";
                
                await connection.ExecuteScalarAsync(sqlQuery, member);
            }
        }

        public async Task DeleteMemberAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"DELETE FROM Members WHERE Id = @memberId";
                
                await connection.ExecuteAsync(sqlQuery, new { memberId });
            }
        }

        public async Task<Member> GetMemberBySaasUserIdAsync(string saasUserId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, Email, IsAfk, IsBanned, LastActivity, LastNudged, Name, PhotoName, Role, SaasUserId, Status  
                    FROM Members
                    WHERE SaasUserId = @saasUserId";

                var data = (await connection.QueryAsync<Member>(sqlQuery, new { saasUserId }))
                    .FirstOrDefault();

                return data;
            }
        }

        public async Task<List<Member>> GetAllMembersByChannelIdAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT m.Id, m.Email, m.IsAfk, m.IsBanned, m.LastActivity, m.LastNudged, m.Name, m.PhotoName, m.Role, m.SaasUserId, m.Status  
                    FROM Members m
                    INNER JOIN ChannelMembers c ON m.Id = c.MemberId
                    WHERE c.ChannelId = @channelId";
                
                var data = (await connection.QueryAsync<Member>(sqlQuery, new { channelId }))
                    .Distinct()
                    .ToList();
                
                return data;
            }
        }
    }
}