// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Chat.Client.SDK.Enums;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel
{
    public class CreateChannelRequest : UserRequest
    {
        public CreateChannelRequest(string saasUserId, string name, ChannelType type)
            : base(saasUserId)
        {
            Name = name;
            Type = type;
            AllowedMembers = new List<string>();
        }

        public string Name { get; }

        public ChannelType Type { get; }

        public string Description { get; set; }

        public string WelcomeMessage { get; set; }

        public List<string> AllowedMembers { get; set; }

        public string PhotoUrl { get; set; }
    }
}