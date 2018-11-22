// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel
{
    public class UpdateChannelRequest : UserRequest
    {
        public UpdateChannelRequest(string saasUserId) : base(saasUserId)
        {
        }

        public Guid ChannelId { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public string WelcomeMessage { get; set; }
        public string PhotoUrl { get; set; }
    }
}