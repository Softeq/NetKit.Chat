// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IDirectChannelService
    {
        Task<DirectChannelResponse> CreateDirectChannel(CreateDirectChannelRequest createDirectChannelRequest);
        Task<DirectChannelResponse> GetDirectChannelById(Guid id);
        Task<DirectMessageResponse> AddMessageAsync(DirectMessage message);
        Task DeleteMessageAsync(Guid id);
        Task UpdateMessageAsync(DirectMessage message);
        Task<IReadOnlyList<DirectMessage>> GetMessagesByChannelIdAsync(Guid channelId);
        Task<DirectMessage> GetMessagesByIdAsync(Guid messageId);
    }
}
