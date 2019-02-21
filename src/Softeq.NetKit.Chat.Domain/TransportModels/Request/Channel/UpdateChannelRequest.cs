// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel
{
    public class UpdateChannelRequest : UserRequest
    {
        public UpdateChannelRequest(string saasUserId, Guid channelId, string name) 
            : base(saasUserId)
        {
            ChannelId = channelId;
            Name = name;
        }

        public Guid ChannelId { get; }

        public string Name { get; }

        public string Description { get; set; }

        public string WelcomeMessage { get; set; }

        public string PhotoUrl { get; set; }
    }
}