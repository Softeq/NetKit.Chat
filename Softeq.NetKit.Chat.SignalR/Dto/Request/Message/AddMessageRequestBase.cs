// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalR.Dto.Request.Message
{
    public abstract class AddMessageRequestBase : BaseRequest
    {
        public Guid ChannelId { get; set; }

        public string Body { get; set; }
    }
}