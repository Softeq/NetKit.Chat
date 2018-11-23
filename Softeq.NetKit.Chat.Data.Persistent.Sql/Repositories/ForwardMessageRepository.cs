
using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
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
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task AddForwardMessageAsync(ForwardMessage message)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"
                    INSERT INTO ForwardMessages(Id, Body, Created, ChannelId, OwnerId) 
                    VALUES (@Id, @Body, @Created, @ChannelId, @OwnerId)";

                await connection.ExecuteScalarAsync(sqlQuery, message);
            }
        }

        public async Task DeleteForwardMessageAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"DELETE FROM ForwardMessages WHERE Id = @messageId";

                await connection.ExecuteAsync(sqlQuery, new { messageId });
            }
        }

        public async Task UpdateForwardMessageAsync(ForwardMessage message)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"UPDATE ForwardMessages 
                                 SET Body = @Body
                                 WHERE Id = @Id";

                await connection.ExecuteAsync(sqlQuery, message);
            }
        }

        public async Task<ForwardMessage> GetForwardMessageByIdAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT *
                    FROM ForwardMessages m INNER JOIN Members mem ON m.OwnerId = mem.Id
                    WHERE m.Id = @messageId";

                var data = (await connection.QueryAsync<ForwardMessage, Member, ForwardMessage>(
                        sqlQuery,
                        (message, member) =>
                        {
                            message.Owner = member;
                            message.OwnerId = member.Id;
                            return message;
                        },
                        new { messageId }))
                    .FirstOrDefault();

                return data;
            }
        }
    }
}
