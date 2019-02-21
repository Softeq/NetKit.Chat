// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Member;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Member
{
    public class InviteMemberRequestValidator : BaseRequestValidator<InviteMemberRequest>
    {
        public InviteMemberRequestValidator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
            RuleFor(x => x.MemberId).NotEmpty();
        }
    }
}