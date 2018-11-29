// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalR.Dto.Request.Channel
{
    public class DeleteMemberRequest : BaseRequest
    {
        public Guid ChannelId { get; set; }

        public Guid MemberId { get; set; }
    }
}