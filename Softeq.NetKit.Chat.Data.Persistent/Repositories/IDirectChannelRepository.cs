// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
    public interface IDirectChannelRepository
    {
        Task CreateDirectChannel(Guid Id, Guid ownerId, Guid memberId);
        Task<DirectChannel> GetDirectChannelById(Guid id);
    }
}
