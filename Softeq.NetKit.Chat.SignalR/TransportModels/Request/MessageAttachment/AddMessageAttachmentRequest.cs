// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.IO;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Request.MessageAttachment
{
    public class AddMessageAttachmentRequest : BaseRequest
    {
        public string SaasUserId { get; set; }

        public Guid MessageId { get; set; }

        public Stream Content { get; set; }

        public string Extension { get; set; }

        public string ContentType { get; set; }

        public long Size { get; set; }
    }
}