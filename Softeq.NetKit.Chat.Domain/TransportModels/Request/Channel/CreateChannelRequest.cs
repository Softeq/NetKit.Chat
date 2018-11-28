// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel
{
    public class CreateChannelRequest
    {
        public CreateChannelRequest(string saasUserId, string clientConnectionId, string name, ChannelType type)
        {
            SaasUserId = saasUserId;
            ClientConnectionId = clientConnectionId;
            Name = name;
            Type = type;
            AllowedMembers = new List<string>();
        }

        public string SaasUserId { get; }

        public string ClientConnectionId { get; }

        public string Name { get; }

        public ChannelType Type { get; }

        public string Description { get; set; }

        public string WelcomeMessage { get; set; }

        public List<string> AllowedMembers { get; set; }

        public string PhotoUrl { get; set; }
    }
}