using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
    public interface IForwardMessageRepository
    {
        Task AddForwardMessageAsync(ForwardMessage message);
        Task DeleteForwardMessageAsync(Guid messageId);
        Task UpdateForwardMessageAsync(ForwardMessage message);
        Task<ForwardMessage> GetForwardMessageByIdAsync(Guid messageId);
    }
}
