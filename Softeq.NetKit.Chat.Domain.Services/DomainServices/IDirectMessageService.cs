﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IDirectMessageService
    {
        Task<DirectChannelResponse> CreateDirectChannel(CreateDirectChannelRequest createDirectChannelRequest);
        Task<DirectChannelResponse> GetDirectChannelById(Guid id);
    }
}
