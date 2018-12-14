// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.MessageAttachment;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.MessageAttachment
{
    public class DeleteMessageAttachmentRequestValidator : BaseRequestValidator<DeleteMessageAttachmentRequest>
    {
        public DeleteMessageAttachmentRequestValidator()
        {
            RuleFor(x => x.MessageId).NotEmpty();
            RuleFor(x => x.AttachmentId).NotEmpty();
        }
    }
}