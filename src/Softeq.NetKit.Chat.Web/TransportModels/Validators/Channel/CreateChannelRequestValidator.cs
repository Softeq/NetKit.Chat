// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.Web.TransportModels.Request.Channel;

namespace Softeq.NetKit.Chat.Web.TransportModels.Validators.Channel
{
    public class CreateChannelRequestValidator : AbstractValidator<CreateChannelRequest>
    {
        public CreateChannelRequestValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
        }
    }
}
