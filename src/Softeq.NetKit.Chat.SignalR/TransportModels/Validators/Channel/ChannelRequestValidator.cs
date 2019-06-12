// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Channel;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Channel
{
    public class ChannelRequestValidator : BaseRequestValidator<SignalRRequest<ChannelRequest>, ChannelRequest>
    {
        public ChannelRequestValidator()
        {
            RuleFor(x => x.Request).NotNull(); RuleFor(x => x.Request).NotNull();
            RuleFor(x => x.Request.ChannelId).NotEmpty();
        }
    }
}