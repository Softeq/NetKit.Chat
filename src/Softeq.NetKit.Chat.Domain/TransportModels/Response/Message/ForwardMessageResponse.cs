// Developed for EPSON AMERICA INC. by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Response.Message
{
    public class ForwardMessageResponse
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public Guid ChannelId { get; set; }
        public Guid? OwnerId { get; set; }
        public ChannelSummaryResponse Channel { get; set; }
        public MemberSummary Owner { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}