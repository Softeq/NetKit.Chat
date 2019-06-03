// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.Web.TransportModels.Request.Channel;

namespace Softeq.NetKit.Chat.Web.TransportModels.Validators.Channel
{
    public class UpdateChannelRequestValidator : AbstractValidator<UpdateChannelRequest>
    {
        public UpdateChannelRequestValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.Name).Must(name => name != null && !name.Contains(' ')).WithMessage(x => $"{nameof(x.Name)} cannot contain spaces.");
        }
    }
}