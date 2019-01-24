// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Request.DirectMessage
{
    public class CreateDirectMembersRequest : BaseRequest
    {
        public Guid OwnerId { get; }
        public Guid MemberId { get; }
    }
}
