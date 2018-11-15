// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Softeq.NetKit.Chat.Data.Interfaces.Repository;
using Softeq.NetKit.Chat.Data.Repositories.Infrastructure;
using Softeq.NetKit.Chat.Domain.Client;
using Softeq.NetKit.Chat.Domain.Member;

namespace Softeq.NetKit.Chat.Data.Repositories.Repositories
{
    internal class ClientRepository : IClientRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public ClientRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<List<Connection>> GetAllClientsAsync()
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, ClientConnectionId, LastActivity, LastClientActivity, Name, UserAgent, MemberId 
                    FROM Connections";
                
                var data = (await connection.QueryAsync<Connection>(sqlQuery)).ToList();

                return data;
            }
        }

        //TODO: Add Unit test
        public async Task<List<Connection>> GetMemberClientsAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, ClientConnectionId, LastActivity, LastClientActivity, Name, UserAgent, MemberId 
                    FROM Connections
                    WHERE MemberId = @memberId";

                var data = (await connection.QueryAsync<Connection>(
                    sqlQuery,
                    new { memberId }))
                    .ToList();
                
                return data;
            }
        }

        public async Task<Connection> GetClientByIdAsync(Guid clientId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT *
                    FROM Connections c 
                    INNER JOIN Members m ON c.MemberId = m.Id
                    WHERE c.Id = @clientId";

                var data = (await connection.QueryAsync<Connection, Member, Connection>(
                        sqlQuery,
                        (client, member) =>
                        {
                            client.Member = member;
                            return client;
                        },
                        new { clientId }))
                    .Distinct()
                    .FirstOrDefault();

                return data;
            }
        }

        public async Task<Connection> GetClientByConnectionIdAsync(string clientConnectionId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT *
                    FROM Connections c 
                    INNER JOIN Members m ON c.MemberId = m.Id
                    WHERE c.ClientConnectionId = @clientConnectionId";

                var data = (await connection.QueryAsync<Connection, Member, Connection>(
                        sqlQuery,
                        (client, member) =>
                        {
                            client.Member = member;
                            return client;
                        },
                        new { clientConnectionId }))
                    .Distinct()
                    .FirstOrDefault();

                return data;
            }
        }

        public async Task AddClientAsync(Connection client)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    INSERT INTO Connections(Id, ClientConnectionId, LastActivity, LastClientActivity, Name, UserAgent, MemberId) 
                    VALUES (@Id, @ClientConnectionId, @LastActivity, @LastClientActivity, @Name, @UserAgent, @MemberId);";
                
                await connection.ExecuteScalarAsync(sqlQuery, client);
            }
        }

        //TODO: Add Unit Test
        public async Task UpdateClientAsync(Connection client)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"UPDATE Connections 
                                 SET Id = @Id, 
                                     ClientConnectionId = @ClientConnectionId, 
                                     LastActivity = @LastActivity, 
                                     LastClientActivity = @LastClientActivity, 
                                     Name = @Name, 
                                     UserAgent = @UserAgent, 
                                     MemberId = @MemberId 
                                 WHERE Id = @Id";
                
                await connection.ExecuteAsync(sqlQuery, client);
            }
        }

        public async Task DeleteClientAsync(Guid clientId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"DELETE FROM Connections WHERE Id = @clientId";

                await connection.ExecuteAsync(sqlQuery, new { clientId });
            }
        }

        public async Task<List<Connection>> GetClientsByMemberIdsAsync(List<Guid> memberIds)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT *
                    FROM Connections c 
                    INNER JOIN Members m ON c.MemberId = m.Id
                    WHERE c.MemberId IN @memberIds";

                var data = (await connection.QueryAsync<Connection, Member, Connection>(
                        sqlQuery,
                        (client, member) =>
                        {
                            client.Member = member;
                            return client;
                        },
                        new { memberIds }))
                    .Distinct()
                    .ToList();

                return data;
            }
        }
    }
}