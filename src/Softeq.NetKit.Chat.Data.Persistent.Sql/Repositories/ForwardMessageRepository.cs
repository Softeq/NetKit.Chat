// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Repositories
{
    internal class ForwardMessageRepository : IForwardMessageRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public ForwardMessageRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task AddForwardMessageAsync(ForwardMessage message)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"INSERT INTO ForwardMessages(Id, Body, Created, ChannelId, OwnerId) 
                                 VALUES (@Id, @Body, @Created, @ChannelId, @OwnerId)";

                await connection.ExecuteScalarAsync(sqlQuery, message);
            }
        }

        public async Task DeleteForwardMessageAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"DELETE FROM ForwardMessages 
                                 WHERE Id = @messageId";

                await connection.ExecuteAsync(sqlQuery, new { messageId });
            }
        }

        public async Task<ForwardMessage> GetForwardMessageAsync(Guid forwardMessageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM ForwardMessages
                                 WHERE Id = @forwardMessageId";

                return (await connection.QueryAsync<ForwardMessage>(sqlQuery, new { forwardMessageId })).FirstOrDefault();
            }
        }
    }
}
