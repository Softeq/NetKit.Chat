// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Enums;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Message;

namespace Softeq.NetKit.Chat.Web.TransportModels.Validators.Message
{
    public class AddMessageRequestValidator : AbstractValidator<AddMessageRequest>
    {
        public AddMessageRequestValidator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
            RuleFor(x => x.Body).NotNull().NotEmpty().When(x => string.IsNullOrEmpty(x.ImageUrl));
            RuleFor(x => x.ForwardedMessageId).NotEmpty().When(x => x.Type == MessageType.Forward);
        }
    }
}
