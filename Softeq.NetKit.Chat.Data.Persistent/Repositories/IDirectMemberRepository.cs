// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
    public interface IDirectMemberRepository
    {
        Task CreateDirectMembers(Guid id, Guid firstMemberId, Guid secondMemberId);
        Task<DirectMembers> GetDirectMembersById(Guid id);
    }
}
