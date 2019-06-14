// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Channel;

namespace Softeq.NetKit.Chat.Web.TransportModels.Validators.Channel
{
    public class CreateDirectChannelRequestValidator: AbstractValidator<CreateDirectChannelRequest>
    {
        public CreateDirectChannelRequestValidator()
        {
            RuleFor(x => x.MemberId).NotEmpty();
        }
    }
}
