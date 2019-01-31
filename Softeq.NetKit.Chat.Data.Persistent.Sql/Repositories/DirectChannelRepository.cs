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
    public class DirectChannelRepository : IDirectChannelRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public DirectChannelRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task CreateDirectChannel(Guid id, Guid ownerId, Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"INSERT INTO DirectChannel(Id, ownerId, MemberId) 
                                 VALUES (@id, @ownerId, @memberId)";

                await connection.ExecuteScalarAsync(sqlQuery, new { id, ownerId, memberId });
            }
        }

        public async Task<DirectChannel> GetDirectChannelById(Guid id)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM DirectChannel
                                 WHERE Id = @id";

                return (await connection.QueryAsync<DirectChannel>(sqlQuery, new { id })).FirstOrDefault();
            }
        }
    }
}
