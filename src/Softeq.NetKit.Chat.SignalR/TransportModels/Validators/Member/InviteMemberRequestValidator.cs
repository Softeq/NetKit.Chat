// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Member;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Member
{
    public class InviteMemberRequestValidator : BaseRequestValidator<SignalRRequest<InviteMemberRequest>, InviteMemberRequest>
    {
        public InviteMemberRequestValidator()
        {
            RuleFor(x => x.Request.ChannelId).NotEmpty();
            RuleFor(x => x.Request.MemberId).NotEmpty();
        }
    }
}