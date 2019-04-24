// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage
{
    public class DirectChannelResponse
    {
        public Guid DirectChannelId { get; set; }
        public MemberSummaryResponse Owner { get; set; }
        public MemberSummaryResponse Member { get; set; }
    }
}
