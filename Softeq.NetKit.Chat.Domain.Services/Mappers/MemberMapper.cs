// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.Services.Configuration;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.Domain.Services.Mappers
{
    internal static class MemberMapper
    {
        public static MemberSummary ToMemberSummary(this DomainModels.Member member, CloudStorageConfiguration configuration)
        {
            var summary = new MemberSummary();
            if (member != null)
            {
                summary.Id = member.Id;
                summary.SaasUserId = member.SaasUserId;
                summary.UserName = member.Name;
                summary.Status = member.Status;
                summary.Role = member.Role;
                summary.IsAfk = member.IsAfk;
                summary.Email = member.Email;
                summary.LastActivity = member.LastActivity;
                summary.AvatarUrl = member.PhotoName != null ? $"{configuration.ContentStorageHost}/{configuration.MemberAvatarsContainer}/{member.PhotoName}" : null;
            }
            return summary;
        }
    }
}