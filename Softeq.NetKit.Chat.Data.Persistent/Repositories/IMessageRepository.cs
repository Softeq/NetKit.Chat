// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Repositories
{
    public interface IMessageRepository
    {
        Task<IReadOnlyCollection<Message>> GetAllChannelMessagesWithOwnersAsync(Guid channelId);
        Task<IReadOnlyCollection<Message>> GetAllDirectChannelMessagesWithOwnersAsync(Guid channelId);
        Task<Message> GetMessageWithOwnerAndForwardMessageAsync(Guid messageId);
        Task<Message> GetPreviousMessageAsync(Guid channelId, Guid? ownerId, DateTimeOffset created);
        Task AddMessageAsync(Message message);
        Task ArchiveMessageAsync(Guid messageId);
        Task UpdateMessageBodyAsync(Guid messageId, string body, DateTimeOffset updated);
        Task<int> GetChannelMessagesCountAsync(Guid channelId);
        Task<Message> GetLastReadMessageAsync(Guid memberId, Guid channelId);
        Task<IReadOnlyCollection<Message>> GetOlderMessagesWithOwnersAsync(Guid channelId, DateTimeOffset lastReadMessageCreated, int? pageSize);
        Task<IReadOnlyCollection<Message>> GetMessagesWithOwnersAsync(Guid channelId, DateTimeOffset lastReadMessageCreated, int? pageSize);
        Task<IReadOnlyCollection<Message>> GetLastMessagesWithOwnersAsync(Guid channelId, DateTimeOffset? lastReadMessageCreated, int pageSize);
        Task<IReadOnlyList<Guid>> FindMessageIdsAsync(Guid channelId, string searchText);
    }
}