// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Request.MessageAttachment
{
    public class DeleteMessageAttachmentRequest : BaseRequest
    {
        public Guid MessageId { get; set; }

        public Guid AttachmentId { get; set; }
    }
}