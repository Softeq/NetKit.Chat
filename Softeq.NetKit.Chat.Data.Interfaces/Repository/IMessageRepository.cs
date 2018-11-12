// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Message;

namespace Softeq.NetKit.Chat.Data.Interfaces.Repository
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetAllChannelMessagesAsync(Guid channelId);
        Task<List<Message>> GetPreviousMessagesAsync(Guid channelId, Guid messageId);
        Task<Message> GetMessageByIdAsync(Guid messageId);
        Task<Message> GetPreviuosMessageAsync(Message currentMessage);
        Task AddMessageAsync(Message message);
        Task DeleteMessageAsync(Guid messageId);
        Task UpdateMessageAsync(Message message);
        Task<int> GetChannelMessagesCountAsync(Guid channelId);
        Task<Message> GetLastReadMessageAsync(Guid memberId, Guid channelId);
        Task<List<Message>> GetOlderMessagesAsync(Guid channelId, DateTimeOffset lastReadMessageCreated, int? pageSize);
        Task<List<Message>> GetMessagesAsync(Guid channelId, DateTimeOffset lastReadMessageCreated, int? pageSize);
        Task<List<Message>> GetLastMessagesAsync(Guid channelId, DateTimeOffset? lastReadMessageCreated, int pageSize = 20);

    }
}