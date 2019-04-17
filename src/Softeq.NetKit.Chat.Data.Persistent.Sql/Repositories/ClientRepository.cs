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
    internal class ClientRepository : IClientRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public ClientRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }
        
        public async Task<IReadOnlyCollection<Client>> GetMemberClientsAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM Clients
                                 WHERE MemberId = @memberId";

                return (await connection.QueryAsync<Client>(sqlQuery, new { memberId })).ToList().AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<string>> GetNotMutedChannelClientConnectionIdsAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT client.ClientConnectionId
                                 FROM Clients client
                                 LEFT JOIN Members member ON client.MemberId = member.Id
                                 LEFT JOIN ChannelMembers channelMember ON member.Id = channelMember.MemberId
                                 WHERE channelMember.ChannelId = @channelId AND channelMember.IsMuted = 0";

                return (await connection.QueryAsync<string>(sqlQuery, new { channelId })).ToList().AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<string>> GetChannelClientConnectionIdsAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT client.ClientConnectionId
                                 FROM Clients client
                                 LEFT JOIN Members member ON client.MemberId = member.Id
                                 LEFT JOIN ChannelMembers channelMember ON member.Id = channelMember.MemberId
                                 WHERE channelMember.ChannelId = @channelId";

                return (await connection.QueryAsync<string>(sqlQuery, new { channelId })).ToList().AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<string>> GetChannelMemberClientConnectionIdsAsync(Guid channelId, Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT client.ClientConnectionId
                                 FROM Clients client
                                 LEFT JOIN Members member ON client.MemberId = member.Id
                                 LEFT JOIN ChannelMembers channelMember ON member.Id = channelMember.MemberId
                                 WHERE channelMember.ChannelId = @channelId AND channelMember.MemberId = @memberId";

                return (await connection.QueryAsync<string>(sqlQuery, new { channelId, memberId })).ToList().AsReadOnly();
            }
        }

        public async Task<Client> GetClientWithMemberAsync(string clientConnectionId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM Clients c 
                                 INNER JOIN Members m ON c.MemberId = m.Id
                                 WHERE c.ClientConnectionId = @clientConnectionId";

                return (await connection.QueryAsync<Client, Member, Client>(sqlQuery,
                        (client, member) =>
                        {
                            client.Member = member;
                            return client;
                        },
                        new { clientConnectionId }))
                    .FirstOrDefault();
            }
        }

        public async Task AddClientAsync(Client client)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"INSERT INTO Clients(Id, ClientConnectionId, LastActivity, LastClientActivity, Name, UserAgent, MemberId) 
                                 VALUES (@Id, @ClientConnectionId, @LastActivity, @LastClientActivity, @Name, @UserAgent, @MemberId)";

                await connection.ExecuteScalarAsync(sqlQuery, client);
            }
        }

        public async Task UpdateClientAsync(Client client)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
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
                var sqlQuery = @"DELETE FROM Clients 
                                 WHERE Id = @clientId";

                await connection.ExecuteAsync(sqlQuery, new { clientId });
            }
        }
        
        public async Task<IReadOnlyCollection<Client>> GetClientsWithMembersAsync(List<Guid> memberIds)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM Clients c 
                                 INNER JOIN Members m ON c.MemberId = m.Id
                                 WHERE c.MemberId IN @memberIds";

                return (await connection.QueryAsync<Client, Member, Client>(sqlQuery,
                        (client, member) =>
                        {
                            client.Member = member;
                            return client;
                        },
                        new { memberIds }))
                    .Distinct()
                    .ToList()
                    .AsReadOnly();
            }
        }

        public async Task<bool> IsClientExistsAsync(string clientConnectionId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT 1
                                 FROM Clients
                                 WHERE ClientConnectionId = @clientConnectionId";

                return await connection.ExecuteScalarAsync<bool>(sqlQuery, new { clientConnectionId });
            }
        }
    }
}