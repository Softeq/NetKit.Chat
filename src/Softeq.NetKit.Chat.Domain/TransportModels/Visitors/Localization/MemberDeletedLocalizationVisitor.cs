// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Localization;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Member;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Message;
using Softeq.NetKit.Chat.Domain.DomainModels.Constants;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Visitors.Localization
{
    public class MemberDeletedLocalizationVisitor : Softeq.NetKit.Chat.Client.SDK.Models.Visitors.Localization.ILocalizationVisitor<MessageResponse>
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
