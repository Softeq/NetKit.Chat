// Developed by Softeq Development Corporation
// http://www.softeq.com

using Dapper;
using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Domain.DomainModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Repositories
{
    public class DirectMembersRepository : IDirectMemberRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public DirectMembersRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task CreateDirectMembers(Guid directId, Guid ownerId, Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"INSERT INTO DirectMembers(Id, OwnerId, MemberId) 
                                 VALUES (@directId, @ownerId, @memberId)";

                await connection.ExecuteScalarAsync(sqlQuery, new { directId, ownerId, memberId });
            }
        }

        public async Task<DirectMembers> GetDirectMembersById(Guid id)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM DirectMembers
                                 WHERE Id = @id";

                return (await connection.QueryAsync<DirectMembers>(sqlQuery, new { id })).FirstOrDefault();
            }
        }
    }
}
