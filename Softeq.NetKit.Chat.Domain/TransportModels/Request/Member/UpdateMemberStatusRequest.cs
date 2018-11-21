// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Member
{
    public class UpdateMemberStatusRequest : UserRequest
    {
        public UpdateMemberStatusRequest(string saasUserId, UserStatus userStatus) : base(saasUserId)
        {
            UserStatus = userStatus;
        }
        public UserStatus UserStatus { get; set; }
    }
}