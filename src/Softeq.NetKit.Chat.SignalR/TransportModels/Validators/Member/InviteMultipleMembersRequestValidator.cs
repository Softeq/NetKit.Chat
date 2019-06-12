// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Member;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Member
{
    public class InviteMultipleMembersRequestValidator : BaseRequestValidator<SignalRRequest<InviteMultipleMembersRequest>, InviteMultipleMembersRequest>
    {
        public InviteMultipleMembersRequestValidator()
        {
            RuleFor(x => x.Request.ChannelId).NotEmpty();
            RuleFor(x => x.Request.InvitedMembersIds).NotNull().NotEmpty();
        }
    }
}