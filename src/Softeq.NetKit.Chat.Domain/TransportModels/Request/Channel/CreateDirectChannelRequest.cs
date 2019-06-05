// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel
{
    public class CreateDirectChannelRequest : UserRequest
    {
        public CreateDirectChannelRequest(string saasUserId, Guid memberId)
            : base(saasUserId)
        {
            MemberId = memberId;
        }

        public Guid MemberId { get; set; }
    }
}