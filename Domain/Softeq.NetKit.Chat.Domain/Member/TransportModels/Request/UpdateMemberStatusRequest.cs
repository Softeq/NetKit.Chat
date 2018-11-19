// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.Member.TransportModels.Request
{
    public class UpdateMemberStatusRequest : UserRequest
    {
        public UpdateMemberStatusRequest(string saasUserId, UserStatus userStatus):base(saasUserId)
        {
            UserStatus = userStatus;
        }
        public UserStatus UserStatus { get; set; }
    }
}