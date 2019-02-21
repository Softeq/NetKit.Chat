// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Request.Message
{
    public class UpdateMessageRequest : BaseRequest
    {
        public Guid MessageId { get; set; }

        public string Body { get; set; }
    }
}