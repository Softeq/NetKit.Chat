// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Message;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Message
{
    public class SetLastReadMessageRequestValidator : BaseRequestValidator<SignalRRequest<SetLastReadMessageRequest>, SetLastReadMessageRequest>
    {
        public SetLastReadMessageRequestValidator()
        {
            RuleFor(x => x.Request.ChannelId).NotEmpty();
            RuleFor(x => x.Request.MessageId).NotEmpty();
        }
    }
}