// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Member;

namespace Softeq.NetKit.Chat.Web.TransportModels.Validators.ChannelMember
{
    public class GetPotentialChannelMembersRequestValidator : AbstractValidator<GetPotentialChannelMembersRequest>
    {
        public GetPotentialChannelMembersRequestValidator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
        }
    }
}
