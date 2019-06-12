// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Member;
using Softeq.NetKit.Chat.TransportModels.Models.SignalRModels;

namespace Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Member
{
    public class DeleteMemberRequestValidator : BaseRequestValidator<SignalRRequest<DeleteMemberRequest>, DeleteMemberRequest>
    {
        public DeleteMemberRequestValidator()
        {
            RuleFor(x => x.Request).NotNull();
            RuleFor(x => x.Request.ChannelId).NotEmpty();
            RuleFor(x => x.Request.MemberId).NotEmpty();
        }
    }
}