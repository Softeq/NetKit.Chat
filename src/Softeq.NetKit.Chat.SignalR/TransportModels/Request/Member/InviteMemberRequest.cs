// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Request.Member
{
    public class InviteMemberRequest : BaseRequest
    {
        public Guid ChannelId { get; set; }

        public Guid MemberId { get; set; }
    }
}