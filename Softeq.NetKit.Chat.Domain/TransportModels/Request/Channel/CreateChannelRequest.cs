// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel
{
    public class CreateChannelRequest : UserRequest
    {
        public CreateChannelRequest(string saasUserId) : base(saasUserId)
        {
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string WelcomeMessage { get; set; }
        public ChannelType Type { get; set; }
        public List<string> AllowedMembers { get; set; }
        public string PhotoUrl { get; set; }
    }
}