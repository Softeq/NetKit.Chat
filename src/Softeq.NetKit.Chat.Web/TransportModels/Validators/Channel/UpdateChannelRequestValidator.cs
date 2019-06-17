// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Channel;

namespace Softeq.NetKit.Chat.Web.TransportModels.Validators.Channel
{
    public class UpdateChannelRequestValidator : AbstractValidator<UpdateChannelRequest>
    {
        public UpdateChannelRequestValidator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
            RuleFor(x => x.Name).NotNull().NotEmpty();
        }
    }
}