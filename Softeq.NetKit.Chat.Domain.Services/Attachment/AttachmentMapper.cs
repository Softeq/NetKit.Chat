// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.Attachment.TransportModels.Response;

namespace Softeq.NetKit.Chat.Domain.Services.Attachment
{
    public static class AttachmentMapper
    {
        public static AttachmentResponse ToAttachmentResponse(this Domain.Attachment.Attachment attachment)
        {
            var attachmentResponse = new AttachmentResponse();
            if (attachment != null)
            {
                attachmentResponse.Id = attachment.Id;
                attachmentResponse.ContentType = attachment.ContentType;
                attachmentResponse.Created = attachment.Created;
                attachmentResponse.FileName = attachment.FileName;
                attachmentResponse.MessageId = attachment.MessageId;
                attachmentResponse.Size = attachment.Size;
            }
            return attachmentResponse;
        }
    }
}