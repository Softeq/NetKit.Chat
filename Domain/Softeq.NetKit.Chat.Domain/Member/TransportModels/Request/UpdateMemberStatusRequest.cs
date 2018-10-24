// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.Member.TransportModels.Request
{
    public class UpdateMemberStatusRequest
    {
        public UpdateMemberStatusRequest(UserStatus userStatus)
        {
            UserStatus = userStatus;
        }

        public string SaasUserId { get; set; }
        public UserStatus UserStatus { get; set; }
    }
}