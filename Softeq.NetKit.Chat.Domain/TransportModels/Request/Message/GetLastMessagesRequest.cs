// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Message
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