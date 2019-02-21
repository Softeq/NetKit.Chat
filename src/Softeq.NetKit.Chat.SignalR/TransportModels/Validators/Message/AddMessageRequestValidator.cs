// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Message;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Message
{
    public class AddMessageRequestValidator : BaseRequestValidator<AddMessageRequest>
    {
        public AddMessageRequestValidator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
            RuleFor(x => x.Body).NotNull().NotEmpty().When(x => string.IsNullOrEmpty(x.ImageUrl));
            RuleFor(x => x.ForwardedMessageId).NotEmpty().When(x => x.Type == MessageType.Forward);
        }
    }
}