// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
   public interface IDirectMessagesRepository
   {
       Task AddMessageAsync(DirectMessage message);
       Task DeleteMessageAsync(Guid id);
       Task UpdateMessageAsync(DirectMessage message);
       Task<IReadOnlyList<DirectMessage>> GetMessagesByChannelIdAsync(Guid channelId);
       Task<DirectMessage> GetMessagesByIdAsync(Guid messageId);

   }
}
