// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Member;

namespace Softeq.NetKit.Chat.Web.TransportModels.Validators.Member
{
    public class InviteMultipleMembersRequestValidator : AbstractValidator<InviteMultipleMembersRequest>
    {
        public InviteMultipleMembersRequestValidator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
            RuleFor(x => x.InvitedMembersIds).NotNull().NotEmpty();
        }
    }
}
