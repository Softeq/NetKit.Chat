// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Request.Message
{
    public class DeleteMessageRequest : BaseRequest
    {
        public Guid MessageId { get; set; }
    }
}