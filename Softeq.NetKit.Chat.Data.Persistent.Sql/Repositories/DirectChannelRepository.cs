﻿// Developed by Softeq Development Corporation
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

        public async Task CreateDirectChannelAsync(Guid directChannelId, Guid ownerId, Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"INSERT INTO DirectChannel(Id, OwnerId, MemberId) 
                                 VALUES (@directChannelId, @ownerId, @memberId)";

                await connection.ExecuteScalarAsync(sqlQuery, new { directChannelId, ownerId, memberId });
            }
        }

        public async Task<DirectChannel> GetDirectChannelAsync(Guid directChannelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM DirectChannel
                                 WHERE Id = @directChannelId";

                return (await connection.QueryAsync<DirectChannel>(sqlQuery, new { directChannelId })).FirstOrDefault();
            }
        }
    }
}
