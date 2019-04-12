// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Response.Message
{
    public class MessageResponse
    {
        public Guid Id { get; set; }
        public Guid ChannelId { get; set; }
        public MemberSummary Sender { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public MessageType Type { get; set; }
        public bool IsRead { get; set; }
        public string ImageUrl { get; set; }
        public ForwardMessage ForwardedMessage { get; set; }
        
        public ChannelTypes ChannelType { get; set; }
    }
}