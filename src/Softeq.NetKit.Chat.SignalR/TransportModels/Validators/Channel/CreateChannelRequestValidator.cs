// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Channel;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Channel
{
    public class CreateChannelRequestValidator : BaseRequestValidator<SignalRRequest<CreateChannelRequest>, CreateChannelRequest>
    {
        public CreateChannelRequestValidator()
        {
            RuleFor(x => x.Request).NotNull();
            RuleFor(x => x.Request.Name).NotNull().NotEmpty();
        }
    }
}