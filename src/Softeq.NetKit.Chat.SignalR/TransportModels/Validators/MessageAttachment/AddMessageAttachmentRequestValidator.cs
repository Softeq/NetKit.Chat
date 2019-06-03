// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.MessageAttachment;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.MessageAttachment
{
    public class AddMessageAttachmentRequestValidator : BaseRequestValidator<AddMessageAttachmentRequest>
    {
        public AddMessageAttachmentRequestValidator()
        {
            RuleFor(x => x.MessageId).NotEmpty();
            RuleFor(x => x.Content).NotNull();
        }
    }
}