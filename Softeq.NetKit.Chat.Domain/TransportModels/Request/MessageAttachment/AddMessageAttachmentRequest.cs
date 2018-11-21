// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.IO;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment
{
    public class AddMessageAttachmentRequest : UserRequest
    {
        public AddMessageAttachmentRequest(string saasUserId, Guid messageId, Stream content, string extension, string contentType, long size)
            : base(saasUserId)
        {
            MessageId = messageId;
            Content = content;
            Extension = extension;
            ContentType = contentType;
            Size = size;
        }

        public Guid MessageId { get; }

        public Stream Content { get; }

        public string Extension { get; }

        public string ContentType { get; }

        public long Size { get; }
    }
}