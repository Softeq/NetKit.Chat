// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalR.Dto.Request.Message
{
    public class AddForwardMessageRequest : AddMessageRequestBase
    {
        public Guid ForwardedMessageId { get; set; }
    }
}