// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.MessageAttachment;

namespace Softeq.NetKit.Chat.Web.TransportModels.Validators.MessageAttachment
{
    public class DeleteMessageAttachmentRequestValidator : AbstractValidator<DeleteMessageAttachmentRequest>
    {
        public DeleteMessageAttachmentRequestValidator()
        {
            RuleFor(x => x.MessageId).NotEmpty();
            RuleFor(x => x.AttachmentId).NotEmpty();
        }
    }
}
