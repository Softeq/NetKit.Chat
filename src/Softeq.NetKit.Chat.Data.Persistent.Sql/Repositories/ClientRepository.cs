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
                        {nameof(Domain.DomainModels.Client.MemberId)} = @{nameof(memberId)}";

                return await connection.ExecuteScalarAsync<bool>(sqlQuery, new { memberId });
            }
        }

        public async Task<IReadOnlyCollection<Domain.DomainModels.Client>> GetMemberClientsAsync(Guid memberId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        {nameof(Domain.DomainModels.Client.Id)},
                        {nameof(Domain.DomainModels.Client.ClientConnectionId)},
                        {nameof(Domain.DomainModels.Client.LastActivity)},
                        {nameof(Domain.DomainModels.Client.LastClientActivity)},
                        {nameof(Domain.DomainModels.Client.Name)},
                        {nameof(Domain.DomainModels.Client.UserAgent)},
                        {nameof(Domain.DomainModels.Client.MemberId)}
                    FROM 
                        Clients
                    WHERE 
                        {nameof(Domain.DomainModels.Client.MemberId)} = @{nameof(memberId)}";

                return (await connection.QueryAsync<Domain.DomainModels.Client>(sqlQuery, new { memberId })).ToList().AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<string>> GetNotMutedChannelClientConnectionIdsAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        client.{nameof(Domain.DomainModels.Client.ClientConnectionId)}
                    FROM 
                        Clients client
                    LEFT JOIN Members member 
                        ON client.{nameof(Domain.DomainModels.Client.MemberId)} = member.{nameof(Member.Id)}
                    LEFT JOIN ChannelMembers channelMember 
                        ON member.{nameof(Member.Id)} = channelMember.{nameof(ChannelMember.MemberId)}
                    WHERE
                        channelMember.{nameof(ChannelMember.ChannelId)} = @{nameof(channelId)}
                        AND channelMember.{nameof(ChannelMember.IsMuted)} = 0";

                return (await connection.QueryAsync<string>(sqlQuery, new { channelId })).ToList().AsReadOnly();
            }
        }

        public async Task<IReadOnlyCollection<string>> GetChannelClientConnectionIdsAsync(Guid channelId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT
                        client.{nameof(Domain.DomainModels.Client.ClientConnectionId)}
                    FROM
                        Clients client
                    LEFT JOIN Members member
                        ON client.{nameof(Domain.DomainModels.Client.MemberId)} = member.{nameof(Member.Id)}
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
                        client.{nameof(Domain.DomainModels.Client.ClientConnectionId)}
                    FROM 
                        Clients client
                    LEFT JOIN Members member 
                        ON client.{nameof(Domain.DomainModels.Client.MemberId)} = member.{nameof(Member.Id)}
                    LEFT JOIN ChannelMembers channelMember 
                        ON member.{nameof(Member.Id)} = channelMember.{nameof(ChannelMember.MemberId)}
                    WHERE 
                        channelMember.{nameof(ChannelMember.ChannelId)} = @{nameof(channelId)} AND channelMember.{nameof(ChannelMember.MemberId)} = @{nameof(memberId)}";

                return (await connection.QueryAsync<string>(sqlQuery, new { channelId, memberId })).ToList().AsReadOnly();
            }
        }

        public async Task<Domain.DomainModels.Client> GetClientWithMemberAsync(string clientConnectionId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        c.{nameof(Domain.DomainModels.Client.Id)},
                        c.{nameof(Domain.DomainModels.Client.ClientConnectionId)},
                        c.{nameof(Domain.DomainModels.Client.LastActivity)},
                        c.{nameof(Domain.DomainModels.Client.LastClientActivity)},
                        c.{nameof(Domain.DomainModels.Client.Name)},
                        c.{nameof(Domain.DomainModels.Client.UserAgent)},
                        c.{nameof(Domain.DomainModels.Client.MemberId)},
                        m.{nameof(Member.Id)},
                        m.{nameof(Member.Email)}, 
                        m.{nameof(Member.IsBanned)},
                        m.{nameof(Member.LastActivity)},
                        m.{nameof(Member.LastNudged)},
                        m.{nameof(Member.Name)},
                        m.{nameof(Member.PhotoName)},
                        m.{nameof(Member.Role)},
                        m.{nameof(Member.SaasUserId)},
                        m.{nameof(Member.Status)},
                        m.{nameof(Member.IsActive)},
                        m.{nameof(Member.IsDeleted)}
                    FROM 
                        Clients c 
                    INNER JOIN Members m 
                        ON c.{nameof(Domain.DomainModels.Client.MemberId)} = m.{nameof(Member.Id)}
                    WHERE 
                        c.{nameof(Domain.DomainModels.Client.ClientConnectionId)} = @{nameof(clientConnectionId)}";

                return (await connection.QueryAsync<Domain.DomainModels.Client, Member, Domain.DomainModels.Client>(sqlQuery,
                        (client, member) =>
                        {
                            client.Member = member;
                            return client;
                        },
                        new { clientConnectionId }))
                    .FirstOrDefault();
            }
        }

        public async Task AddClientAsync(Domain.DomainModels.Client client)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    INSERT INTO Clients
                    (
                        {nameof(Domain.DomainModels.Client.Id)},
                        {nameof(Domain.DomainModels.Client.ClientConnectionId)}, 
                        {nameof(Domain.DomainModels.Client.LastActivity)}, 
                        {nameof(Domain.DomainModels.Client.LastClientActivity)}, 
                        {nameof(Domain.DomainModels.Client.Name)}, 
                        {nameof(Domain.DomainModels.Client.UserAgent)}, 
                        {nameof(Domain.DomainModels.Client.MemberId)}
                    ) VALUES 
                    (
                        @{nameof(Domain.DomainModels.Client.Id)},
                        @{nameof(Domain.DomainModels.Client.ClientConnectionId)}, 
                        @{nameof(Domain.DomainModels.Client.LastActivity)}, 
                        @{nameof(Domain.DomainModels.Client.LastClientActivity)}, 
                        @{nameof(Domain.DomainModels.Client.Name)}, 
                        @{nameof(Domain.DomainModels.Client.UserAgent)}, 
                        @{nameof(Domain.DomainModels.Client.MemberId)}
                    )";

                await connection.ExecuteScalarAsync(sqlQuery, client);
            }
        }

        public async Task UpdateClientAsync(Domain.DomainModels.Client client)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    UPDATE Clients 
                    SET 
                        {nameof(Domain.DomainModels.Client.Id)} = @{nameof(Domain.DomainModels.Client.Id)}, 
                        {nameof(Domain.DomainModels.Client.ClientConnectionId)} = @{nameof(Domain.DomainModels.Client.ClientConnectionId)}, 
                        {nameof(Domain.DomainModels.Client.LastActivity)} = @{nameof(Domain.DomainModels.Client.LastActivity)}, 
                        {nameof(Domain.DomainModels.Client.LastClientActivity)} = @{nameof(Domain.DomainModels.Client.LastClientActivity)}, 
                        {nameof(Domain.DomainModels.Client.Name)} = @{nameof(Domain.DomainModels.Client.Name)}, 
                        {nameof(Domain.DomainModels.Client.UserAgent)} = @{nameof(Domain.DomainModels.Client.UserAgent)}, 
                        {nameof(Domain.DomainModels.Client.MemberId)} = @{nameof(Domain.DomainModels.Client.MemberId)}
                    WHERE 
                        {nameof(Domain.DomainModels.Client.Id)} = @{nameof(Domain.DomainModels.Client.Id)}";

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
                        {nameof(Domain.DomainModels.Client.Id)} = @{nameof(clientId)}";

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
                        {nameof(Domain.DomainModels.Client.MemberId)} = @{nameof(memberId)} AND DATEDIFF(MINUTE, {nameof(Domain.DomainModels.Client.LastClientActivity)}, GETUTCDATE()) > @{nameof(inactiveMinutesThreshold)}";

                await connection.ExecuteAsync(sqlQuery, new { memberId, inactiveMinutesThreshold });
            }
        }

        public async Task<IReadOnlyCollection<Domain.DomainModels.Client>> GetClientsWithMembersAsync(List<Guid> memberIds)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT
                        c.{nameof(Domain.DomainModels.Client.Id)},
                        c.{nameof(Domain.DomainModels.Client.ClientConnectionId)},
                        c.{nameof(Domain.DomainModels.Client.LastActivity)},
                        c.{nameof(Domain.DomainModels.Client.LastClientActivity)},
                        c.{nameof(Domain.DomainModels.Client.Name)},
                        c.{nameof(Domain.DomainModels.Client.UserAgent)},
                        c.{nameof(Domain.DomainModels.Client.MemberId)},
                        m.{nameof(Member.Id)},
                        m.{nameof(Member.Email)},
                        m.{nameof(Member.IsBanned)},
                        m.{nameof(Member.LastActivity)},
                        m.{nameof(Member.LastNudged)},
                        m.{nameof(Member.Name)},
                        m.{nameof(Member.PhotoName)},
                        m.{nameof(Member.Role)},
                        m.{nameof(Member.SaasUserId)},
                        m.{nameof(Member.Status)},
                        m.{nameof(Member.IsActive)},
                        m.{nameof(Member.IsDeleted)}
                    FROM 
                        Clients c 
                    INNER JOIN Members m 
                        ON c.{nameof(Domain.DomainModels.Client.MemberId)} = m.{nameof(Member.Id)}
                    WHERE 
                        c.{nameof(Domain.DomainModels.Client.MemberId)} IN @{nameof(memberIds)}";

                return (await connection.QueryAsync<Domain.DomainModels.Client, Member, Domain.DomainModels.Client>(sqlQuery,
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
                    WHERE {nameof(Domain.DomainModels.Client.ClientConnectionId)} = @{nameof(clientConnectionId)}";

                return await connection.ExecuteScalarAsync<bool>(sqlQuery, new { clientConnectionId });
            }
        }
    }
}