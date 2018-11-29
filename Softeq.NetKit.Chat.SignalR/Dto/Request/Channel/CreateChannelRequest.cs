// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.SignalR.Dto.Request.Channel
{
    public class CreateChannelRequest : BaseRequest
    {
        public CreateChannelRequest()
        {
            AllowedMembers = new List<string>();
        }

        public string Name { get; set; }

        public ChannelType Type { get; set; }

        public string Description { get; set; }

        public string WelcomeMessage { get; set; }

        public List<string> AllowedMembers { get; set; }

        public string PhotoUrl { get; set; }
    }
}