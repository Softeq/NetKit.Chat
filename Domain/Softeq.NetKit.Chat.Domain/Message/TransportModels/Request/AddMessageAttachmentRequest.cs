// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.IO;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;

namespace Softeq.NetKit.Chat.Domain.Message.TransportModels.Request
{
    public class AddMessageAttachmentRequest : UserRequest
    {
        public AddMessageAttachmentRequest(string saasUserId, Guid messageId, Stream content, string extension, string contentType, long size) : base(saasUserId) 
        {
            MessageId = messageId;
            Content = content;
            Extension = extension;
            ContentType = contentType;
            Size = size;
        }

        public Guid MessageId { get; set; }
        public Stream Content { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
    }
}