// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;

namespace Softeq.NetKit.Chat.Domain.Message.TransportModels.Request
{
    public class GetLastMessagesRequest : UserRequest
    {
        public GetLastMessagesRequest(string userId, Guid channelId) : base(userId)
        {
            ChannelId = channelId;
        }

        public Guid ChannelId { get; set; }
    }
}