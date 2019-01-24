// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Request.DirectMessage
{
    public class CreateDirectMembersRequest : BaseRequest
    {
        public Guid FirstMemberId { get; }
        public Guid SecondMemberId { get; }
    }
}
