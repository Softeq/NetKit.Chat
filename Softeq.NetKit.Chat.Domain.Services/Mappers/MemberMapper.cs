// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.Services.Configuration;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.Domain.Services.Mappers
{
    internal static class MemberMapper
    {
        public static ParticipantResponse ToParticipantResponse(this DomainModels.Member member)
        {
            var participant = new ParticipantResponse();
            if (member != null)
            {
                participant.Id = member.Id;
                participant.Email = member.Email;
                participant.Role = member.Role;
                participant.IsAfk = member.IsAfk;
                participant.IsBanned = member.IsBanned;
                participant.LastActivity = member.LastActivity;
                participant.LastNudged = member.LastNudged;
                participant.SaasUserId = member.SaasUserId;
                participant.UserName = member.Name;
                participant.Status = member.Status;
            }
            return participant;
        }

        public static DomainModels.Member ToMember(this ParticipantResponse response)
        {
            var member = new DomainModels.Member();
            if (response != null)
            {
                member.Id = response.Id;
                member.Email = response.Email;
                member.Role = response.Role;
                member.IsAfk = response.IsAfk;
                member.IsBanned = response.IsBanned;
                member.LastActivity = response.LastActivity;
                member.LastNudged = response.LastNudged;
                member.SaasUserId = response.SaasUserId;
                member.Name = response.UserName;
                member.Status = response.Status;
            }
            return member;
        }

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