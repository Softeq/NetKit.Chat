// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using Softeq.NetKit.Chat.Domain.Member.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Services.App.Configuration;

namespace Softeq.NetKit.Chat.Domain.Services.Member
{
    internal static class MemberMapper
    {
        public static ParticipantResponse ToParticipantResponse(this Domain.Member.Member member)
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

        public static Domain.Member.Member ToMember(this ParticipantResponse response)
        {
            var member = new Domain.Member.Member();
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

        public static MemberSummary ToMemberSummary(this Domain.Member.Member member,
            CloudStorageConfiguration configuration)
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
                summary.AvatarUrl = member.PhotoName != null
                    ? $"{configuration.ContentStorageHost}/{configuration.MemberAvatarsContainer}/{member.PhotoName}"
                    : null;
            }

            return summary;
        }
    }
}