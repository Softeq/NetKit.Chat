// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
    public interface IForwardMessageRepository
    {
        Task AddForwardMessageAsync(ForwardMessage message);
        Task DeleteForwardMessageAsync(Guid messageId);
        Task<ForwardMessage> GetForwardMessageAsync(Guid forwardMessageId);
    }
}
