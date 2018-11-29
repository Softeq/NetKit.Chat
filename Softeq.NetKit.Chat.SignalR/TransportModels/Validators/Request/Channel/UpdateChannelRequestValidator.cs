// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Channel;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Request.Channel
{
    public class UpdateChannelRequestValidator : AbstractValidator<UpdateChannelRequest>
    {
        public UpdateChannelRequestValidator()
        {
            RuleFor(x => x.Name).NotNull();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Name).Must(name => !name.Contains(' ')).WithMessage("Room name cannot contain spaces.");
        }
    }
}