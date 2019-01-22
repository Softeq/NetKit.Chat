// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
    public interface IDirectMemberRepository
    {
        Task CreateDirectMembers(Guid id, Guid member01Id, Guid member02Id);
        Task<DirectMembers> GetDirectMembersById(Guid id);
    }
}
