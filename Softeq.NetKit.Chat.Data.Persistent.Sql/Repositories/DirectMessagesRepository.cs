// Developed by Softeq Development Corporation
// http://www.softeq.com

using Dapper;
using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Repositories
{
    public class DirectMessagesRepository : IDirectMessagesRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public DirectMessagesRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task AddMessageAsync(DirectMessage message)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"INSERT INTO DirectMessages(Id, DirectChannelId, Created, OwnerId, Body, Updated) 
                                 VALUES (@Id, @DirectChannelId, @Created, @OwnerId, @Body, @Updated)";

                await connection.ExecuteScalarAsync(sqlQuery, message);
            }
        }

        public async Task DeleteMessageAsync(Guid id)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"DELETE FROM DirectMessages WHERE Id = @Id";

                await connection.ExecuteScalarAsync(sqlQuery, new { id });
            }
        }

        public async Task UpdateMessageAsync(DirectMessage message)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"UPDATE DirectMessages 
                                 SET Body = @Body, 
                                     Updated = @Updated                                     
                                 WHERE Id = @Id";

                await connection.ExecuteScalarAsync(sqlQuery, new { message.Body, message.Updated, message.Id });
            }
        }

        public async Task<IReadOnlyList<DirectMessage>> GetMessagesByChannelIdAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *  
                                 FROM DirectMessages                           
                                 WHERE DirectChannelId = @channelId
                                 ORDER BY Created DESC";

                return (await connection.QueryAsync<DirectMessage>(sqlQuery, new { channelId })).ToList().AsReadOnly();
            }
        }

        public async Task<DirectMessage> GetMessageByIdAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *  
                                 FROM DirectMessages                           
                                 WHERE Id = @messageId";

                return (await connection.QueryAsync<DirectMessage>(sqlQuery, new { messageId })).FirstOrDefault();
            }
        }
    }
}
