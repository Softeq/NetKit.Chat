// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.Client.SDK.Enums;
using Softeq.NetKit.Chat.Web.TransportModels.Request.Message;

namespace Softeq.NetKit.Chat.Web.TransportModels.Validators.Message
{
    public class AddMessageRequestValidator : AbstractValidator<AddMessageRequest>
    {
        public AddMessageRequestValidator()
        {
            RuleFor(x => x.ClientConnectionId).NotNull().NotEmpty();
            RuleFor(x => x.Body).NotNull().NotEmpty();
            RuleFor(x => x.ForwardedMessageId).NotEmpty().When(customer => customer.Type == MessageType.Forward);
        }
    }
}
