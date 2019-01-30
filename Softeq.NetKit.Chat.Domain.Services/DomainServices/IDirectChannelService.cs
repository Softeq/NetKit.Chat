﻿// Developed by Softeq Development Corporation
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
        Task<DirectMessageResponse> UpdateMessageAsync(DirectMessage message);
        Task<IList<DirectMessageResponse>> GetMessagesByChannelIdAsync(Guid channelId);
        Task<DirectMessageResponse> GetMessagesByIdAsync(Guid messageId);
    }
}
