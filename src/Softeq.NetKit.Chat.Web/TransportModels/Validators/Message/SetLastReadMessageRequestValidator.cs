// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Message;

namespace Softeq.NetKit.Chat.Web.TransportModels.Validators.Message
{
    public class SetLastReadMessageRequestValidator : AbstractValidator<SetLastReadMessageRequest>
    {
        public SetLastReadMessageRequestValidator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
            RuleFor(x => x.MessageId).NotEmpty();
        }
    }
}
