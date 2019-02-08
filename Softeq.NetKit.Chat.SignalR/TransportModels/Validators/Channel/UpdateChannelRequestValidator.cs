// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using FluentValidation;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Channel;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Channel
{
    public class UpdateChannelRequestValidator : BaseRequestValidator<UpdateChannelRequest>
    {
        public UpdateChannelRequestValidator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.Name).Must(name => name != null && !name.Contains(' ')).WithMessage(x => $"{nameof(x.Name)} cannot contain spaces.");
        }
    }
}