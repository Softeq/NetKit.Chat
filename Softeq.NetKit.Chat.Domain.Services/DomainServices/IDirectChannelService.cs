// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IDirectChannelService
    {
        Task<DirectChannelResponse> CreateDirectChannel(CreateDirectChannelRequest createDirectChannelRequest);
        Task<DirectChannelResponse> GetDirectChannelById(Guid id);
        Task<DirectMessageResponse> AddMessageAsync(CreateDirectMessageRequest request);
        Task<DirectMessageResponse> DeleteMessageAsync(Guid messageId, string saasUserId);
        Task<DirectMessageResponse> UpdateMessageAsync(UpdateDirectMessageRequest request);
        Task<IList<DirectMessageResponse>> GetMessagesByChannelIdAsync(Guid channelId);
        Task<DirectMessageResponse> GetMessagesByIdAsync(Guid messageId);
    }
}
