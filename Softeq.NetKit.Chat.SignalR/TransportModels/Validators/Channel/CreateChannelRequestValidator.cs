// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Channel;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Channel
{
    public class CreateChannelRequestValidator : BaseRequestValidator<CreateChannelRequest>
    {
        public CreateChannelRequestValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
        }
    }
}