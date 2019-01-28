// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Web.TransportModels.Request.DirectChannel
{
    public class CreateDirectChannelRequest
    {
        public CreateDirectChannelRequest(Guid ownerId, Guid memberId)
        {
            OwnerId = ownerId;
            MemberId = memberId;
        }

        public Guid OwnerId { get; }
        public Guid MemberId { get; }
    }
}
