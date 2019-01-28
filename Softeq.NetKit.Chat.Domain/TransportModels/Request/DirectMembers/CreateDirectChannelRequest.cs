// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectMembers
{
    public class CreateDirectChannelRequest : UserRequest
    {
        public CreateDirectChannelRequest(string saasUserId, Guid ownerId, Guid memberId)
            : base(saasUserId)
        {
            OwnerId = ownerId;
            MemberId = memberId;
        }

        public Guid DirectMembersId { get; set; }
        public Guid OwnerId { get; }
        public Guid MemberId { get; }
    }
}
