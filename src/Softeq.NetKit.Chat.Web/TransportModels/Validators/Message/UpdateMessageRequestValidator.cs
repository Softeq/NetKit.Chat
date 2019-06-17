﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Message;

namespace Softeq.NetKit.Chat.Web.TransportModels.Validators.Message
{
    public class UpdateMessageRequestValidator : AbstractValidator<UpdateMessageRequest>
    {
        public UpdateMessageRequestValidator()
        {
            RuleFor(x => x.Body).NotNull().NotEmpty();
        }
    }
}