// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.MessageAttachment;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.MessageAttachment
{
    public class DeleteMessageAttachmentRequestValidator : BaseRequestValidator<SignalRRequest<DeleteMessageAttachmentRequest>, DeleteMessageAttachmentRequest>
    {
        public DeleteMessageAttachmentRequestValidator()
        {
            RuleFor(x => x.Request).NotNull();
            RuleFor(x => x.Request.MessageId).NotEmpty();
            RuleFor(x => x.Request.AttachmentId).NotEmpty();
        }
    }
}