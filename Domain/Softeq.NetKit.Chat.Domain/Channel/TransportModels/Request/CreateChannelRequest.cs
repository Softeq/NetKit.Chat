// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;

namespace Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request
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