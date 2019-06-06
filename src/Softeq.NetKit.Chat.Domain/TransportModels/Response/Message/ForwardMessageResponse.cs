// Developed for EPSON AMERICA INC. by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Response.Message
{
    public class ForwardMessageResponse
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public Guid ChannelId { get; set; }
        public Guid? OwnerId { get; set; }
        public ChannelSummaryResponse Channel { get; set; }
        public MemberSummaryResponse Owner { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}