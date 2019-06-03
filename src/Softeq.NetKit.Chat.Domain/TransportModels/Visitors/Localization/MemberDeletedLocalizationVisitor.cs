// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Chat.Domain.DomainModels.Constants;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Localization;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Visitors.Localization
{
    public class MemberDeletedLocalizationVisitor : ILocalizationVisitor<MessageResponse>
    {
        private readonly MemberSummaryResponse _member;

        public MemberDeletedLocalizationVisitor(MemberSummaryResponse member)
        {
            _member = member;
        }

        public void Visit(MessageResponse entity)
        {
            entity.Localization = new LocalizationResponse
            {
                Key = LocalizationKeys.SystemMemberDeleted,
                Parameters = new Dictionary<string, string>
                {
                    [nameof(_member.UserName)] = _member.UserName
                }
            };
        }
    }
}
