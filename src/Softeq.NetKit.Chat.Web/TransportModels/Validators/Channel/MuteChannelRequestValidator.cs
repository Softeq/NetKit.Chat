﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Channel;

namespace Softeq.NetKit.Chat.Web.TransportModels.Validators.Channel
{
    public class MuteChannelRequestValidator : AbstractValidator<MuteChannelRequest>
    {
        public MuteChannelRequestValidator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
        }
    }
}
