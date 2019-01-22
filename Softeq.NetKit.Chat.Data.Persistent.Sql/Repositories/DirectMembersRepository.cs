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
    public class DirectMembersRepository : IDirectMemberRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public DirectMembersRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public Task CreateDirectMembers(Guid id, Guid member01Id, Guid member02Id)
        {
            throw new NotImplementedException();
        }

        public Task<DirectMembers> GetDirectMembersById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
