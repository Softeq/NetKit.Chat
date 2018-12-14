// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Channel;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Channel
{
    public class PinChannelRequestValidator : BaseRequestValidator<PinChannelRequest>
    {
        public PinChannelRequestValidator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
        }
    }
}