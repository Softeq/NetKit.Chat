// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Enums;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Message;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Message
{
    public class AddMessageRequestValidator : BaseRequestValidator<SignalRRequest<AddMessageRequest>, AddMessageRequest>
    {
        public AddMessageRequestValidator()
        {
            RuleFor(x => x.Request).NotNull();
            RuleFor(x => x.Request.ChannelId).NotEmpty();
            RuleFor(x => x.Request.Body).NotNull().NotEmpty().When(x => string.IsNullOrEmpty(x.Request.ImageUrl));
            RuleFor(x => x.Request.ForwardedMessageId).NotEmpty().When(x => x.Request.Type == MessageType.Forward);
        }
    }
}