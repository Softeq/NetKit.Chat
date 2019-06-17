// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Member;

namespace Softeq.NetKit.Chat.Web.TransportModels.Validators.Member
{
    public class DeleteMemberRequestValidator : AbstractValidator<DeleteMemberRequest>
    {
        public DeleteMemberRequestValidator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
            RuleFor(x => x.MemberId).NotEmpty();
        }
    }
}
