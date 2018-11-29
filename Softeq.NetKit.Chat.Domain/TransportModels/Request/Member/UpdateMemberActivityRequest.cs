// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Member
{
    public class UpdateMemberActivityRequest : UserRequest
    {
        public UpdateMemberActivityRequest(string saasUserId, string connectionId, string userAgent)
            : base(saasUserId)
        {
            ConnectionId = connectionId;
            UserAgent = userAgent;
        }

        public string ConnectionId { get; }

        public string UserAgent { get; }
    }
}