// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Member;

namespace Softeq.NetKit.Chat.Web.TransportModels.Validators.Member
{
    public class InviteMemberRequestValidator : AbstractValidator<InviteMemberRequest>
    {
        public InviteMemberRequestValidator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
            RuleFor(x => x.MemberId).NotEmpty();
        }
    }
}
