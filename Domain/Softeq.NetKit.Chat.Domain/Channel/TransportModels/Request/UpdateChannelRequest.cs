// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;

namespace Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request
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