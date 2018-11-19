// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
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

        public async Task<List<Client>> GetAllClientsAsync()
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, ClientConnectionId, LastActivity, LastClientActivity, Name, UserAgent, MemberId 
                    FROM Clients";
                
                var data = (await connection.QueryAsync<Client>(sqlQuery)).ToList();

                return data;
            }
        }

        //TODO: Add Unit test
        public async Task<List<Client>> GetMemberClientsAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT Id, ClientConnectionId, LastActivity, LastClientActivity, Name, UserAgent, MemberId 
                    FROM Clients
                    WHERE MemberId = @memberId";

                var data = (await connection.QueryAsync<Client>(
                    sqlQuery,
                    new { memberId }))
                    .ToList();
                
                return data;
            }
        }

        public async Task<Client> GetClientByIdAsync(Guid clientId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT *
                    FROM Clients c 
                    INNER JOIN Members m ON c.MemberId = m.Id
                    WHERE c.Id = @clientId";

                var data = (await connection.QueryAsync<Client, Member, Client>(
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

        public async Task<Client> GetClientByConnectionIdAsync(string clientConnectionId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT *
                    FROM Clients c 
                    INNER JOIN Members m ON c.MemberId = m.Id
                    WHERE c.ClientConnectionId = @clientConnectionId";

                var data = (await connection.QueryAsync<Client, Member, Client>(
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

        public async Task AddClientAsync(Client client)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    INSERT INTO Clients(Id, ClientConnectionId, LastActivity, LastClientActivity, Name, UserAgent, MemberId) 
                    VALUES (@Id, @ClientConnectionId, @LastActivity, @LastClientActivity, @Name, @UserAgent, @MemberId);";
                
                await connection.ExecuteScalarAsync(sqlQuery, client);
            }
        }

        //TODO: Add Unit Test
        public async Task UpdateClientAsync(Client client)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"UPDATE Clients 
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

                var sqlQuery = @"DELETE FROM Clients WHERE Id = @clientId";

                await connection.ExecuteAsync(sqlQuery, new { clientId });
            }
        }

        public async Task<List<Client>> GetClientsByMemberIdsAsync(List<Guid> memberIds)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                var sqlQuery = @"
                    SELECT *
                    FROM Clients c 
                    INNER JOIN Members m ON c.MemberId = m.Id
                    WHERE c.MemberId IN @memberIds";

                var data = (await connection.QueryAsync<Client, Member, Client>(
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