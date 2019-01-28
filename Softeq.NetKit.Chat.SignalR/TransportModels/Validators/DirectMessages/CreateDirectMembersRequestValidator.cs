﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.DirectMessage;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.DirectMessages
{
    public class CreateDirectMembersRequestValidator : BaseRequestValidator<CreateDirectMembersRequest>
    {
        public CreateDirectMembersRequestValidator()
        {
            RuleFor(x => x.OwnerId).NotNull().NotEmpty();
            RuleFor(x => x.MemberId).NotNull().NotEmpty();
        }
    }
}