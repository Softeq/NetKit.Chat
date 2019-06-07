// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Repositories
{
    internal class ClientRepository : BaseRepository, IClientRepository
    {
        public ClientRepository(ISqlConnectionFactory sqlConnectionFactory) : base(sqlConnectionFactory)
        {
        }

        public async Task<bool> DoesMemberHasClientsAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        1
                    FROM 
                        Clients
                    WHERE 
                        {nameof(Client.MemberId)} = @{nameof(memberId)}";

                return await connection.ExecuteScalarAsync<bool>(sqlQuery, new { memberId });
            }
        }

        public async Task<IReadOnlyCollection<Client>> GetMemberClientsAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        {nameof(Client.Id)},
                        {nameof(Client.ClientConnectionId)},
                        {nameof(Client.LastActivity)},
                        {nameof(Client.LastClientActivity)},
                        {nameof(Client.Name)},
                        {nameof(Client.UserAgent)},
                        {nameof(Client.MemberId)}
                    FROM 
                        Clients
                    WHERE 
                        {nameof(Client.MemberId)} = @{nameof(memberId)}";

                return (await connection.QueryAsync<Client>(sqlQuery, new { memberId })).ToList().AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<string>> GetChannelClientConnectionIdsAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT
                        client.{nameof(Client.ClientConnectionId)}
                    FROM
                        Clients client
                    LEFT JOIN Members member
                        ON client.{nameof(Client.MemberId)} = member.{nameof(Member.Id)}
                    LEFT JOIN ChannelMembers channelMember
                        ON member.{nameof(Member.Id)} = channelMember.{nameof(ChannelMember.MemberId)}
                    WHERE
                        channelMember.{nameof(ChannelMember.ChannelId)} = @{nameof(channelId)}";

                return (await connection.QueryAsync<string>(sqlQuery, new { channelId })).ToList().AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<string>> GetChannelMemberClientConnectionIdsAsync(Guid channelId, Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        client.{nameof(Client.ClientConnectionId)}
                    FROM 
                        Clients client
                    LEFT JOIN Members member 
                        ON client.{nameof(Client.MemberId)} = member.{nameof(Member.Id)}
                    LEFT JOIN ChannelMembers channelMember 
                        ON member.{nameof(Member.Id)} = channelMember.{nameof(ChannelMember.MemberId)}
                    WHERE 
                        channelMember.{nameof(ChannelMember.ChannelId)} = @{nameof(channelId)} AND channelMember.{nameof(ChannelMember.MemberId)} = @{nameof(memberId)}";

                return (await connection.QueryAsync<string>(sqlQuery, new { channelId, memberId })).ToList().AsReadOnly();
            }
        }

        public async Task<Client> GetClientWithMemberAsync(string clientConnectionId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        c.{nameof(Client.Id)},
                        c.{nameof(Client.ClientConnectionId)},
                        c.{nameof(Client.LastActivity)},
                        c.{nameof(Client.LastClientActivity)},
                        c.{nameof(Client.Name)},
                        c.{nameof(Client.UserAgent)},
                        c.{nameof(Client.MemberId)},
                        m.{nameof(Member.Id)},
                        m.{nameof(Member.Email)}, 
                        m.{nameof(Member.IsBanned)},
                        m.{nameof(Member.LastActivity)},
                        m.{nameof(Member.LastNudged)},
                        m.{nameof(Member.Name)},
                        m.{nameof(Member.PhotoName)},
                        m.{nameof(Member.SaasUserId)},
                        m.{nameof(Member.Status)},
                        m.{nameof(Member.IsActive)},
                        m.{nameof(Member.IsDeleted)}
                    FROM 
                        Clients c 
                    INNER JOIN Members m 
                        ON c.{nameof(Client.MemberId)} = m.{nameof(Member.Id)}
                    WHERE 
                        c.{nameof(Client.ClientConnectionId)} = @{nameof(clientConnectionId)}";

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
                var sqlQuery = $@"
                    INSERT INTO Clients
                    (
                        {nameof(Client.Id)},
                        {nameof(Client.ClientConnectionId)}, 
                        {nameof(Client.LastActivity)}, 
                        {nameof(Client.LastClientActivity)}, 
                        {nameof(Client.Name)}, 
                        {nameof(Client.UserAgent)}, 
                        {nameof(Client.MemberId)}
                    ) VALUES 
                    (
                        @{nameof(Client.Id)},
                        @{nameof(Client.ClientConnectionId)}, 
                        @{nameof(Client.LastActivity)}, 
                        @{nameof(Client.LastClientActivity)}, 
                        @{nameof(Client.Name)}, 
                        @{nameof(Client.UserAgent)}, 
                        @{nameof(Client.MemberId)}
                    )";

                await connection.ExecuteScalarAsync(sqlQuery, client);
            }
        }

        public async Task UpdateClientAsync(Client client)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    UPDATE Clients 
                    SET 
                        {nameof(Client.Id)} = @{nameof(Client.Id)}, 
                        {nameof(Client.ClientConnectionId)} = @{nameof(Client.ClientConnectionId)}, 
                        {nameof(Client.LastActivity)} = @{nameof(Client.LastActivity)}, 
                        {nameof(Client.LastClientActivity)} = @{nameof(Client.LastClientActivity)}, 
                        {nameof(Client.Name)} = @{nameof(Client.Name)}, 
                        {nameof(Client.UserAgent)} = @{nameof(Client.UserAgent)}, 
                        {nameof(Client.MemberId)} = @{nameof(Client.MemberId)}
                    WHERE 
                        {nameof(Client.Id)} = @{nameof(Client.Id)}";

                await connection.ExecuteAsync(sqlQuery, client);
            }
        }

        public async Task DeleteClientAsync(Guid clientId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    DELETE FROM Clients 
                    WHERE 
                        {nameof(Client.Id)} = @{nameof(clientId)}";

                await connection.ExecuteAsync(sqlQuery, new { clientId });
            }
        }

        public async Task DeleteOverThresholdMemberClientsAsync(Guid memberId, int inactiveMinutesThreshold)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    DELETE FROM Clients 
                    WHERE 
                        {nameof(Client.MemberId)} = @{nameof(memberId)} AND DATEDIFF(MINUTE, {nameof(Client.LastClientActivity)}, GETUTCDATE()) > @{nameof(inactiveMinutesThreshold)}";

                await connection.ExecuteAsync(sqlQuery, new { memberId, inactiveMinutesThreshold });
            }
        }

        public async Task<IReadOnlyCollection<Client>> GetClientsWithMembersAsync(List<Guid> memberIds)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT
                        c.{nameof(Client.Id)},
                        c.{nameof(Client.ClientConnectionId)},
                        c.{nameof(Client.LastActivity)},
                        c.{nameof(Client.LastClientActivity)},
                        c.{nameof(Client.Name)},
                        c.{nameof(Client.UserAgent)},
                        c.{nameof(Client.MemberId)},
                        m.{nameof(Member.Id)},
                        m.{nameof(Member.Email)},
                        m.{nameof(Member.IsBanned)},
                        m.{nameof(Member.LastActivity)},
                        m.{nameof(Member.LastNudged)},
                        m.{nameof(Member.Name)},
                        m.{nameof(Member.PhotoName)},
                        m.{nameof(Member.SaasUserId)},
                        m.{nameof(Member.Status)},
                        m.{nameof(Member.IsActive)},
                        m.{nameof(Member.IsDeleted)}
                    FROM 
                        Clients c 
                    INNER JOIN Members m 
                        ON c.{nameof(Client.MemberId)} = m.{nameof(Member.Id)}
                    WHERE 
                        c.{nameof(Client.MemberId)} IN @{nameof(memberIds)}";

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
                var sqlQuery = $@"
                    SELECT 
                        1
                    FROM 
                        Clients
                    WHERE {nameof(Client.ClientConnectionId)} = @{nameof(clientConnectionId)}";

                return await connection.ExecuteScalarAsync<bool>(sqlQuery, new { clientConnectionId });
            }
        }
    }
}