// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Channel;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Channel
{
    public class CreateDirectChannelRequestValidator : BaseRequestValidator<SignalRRequest<CreateDirectChannelRequest>, CreateDirectChannelRequest>
    {
        public CreateDirectChannelRequestValidator()
        {
            RuleFor(x => x.Request).NotNull();
            RuleFor(x => x.Request.MemberId).NotEmpty();
        }
    }
}
