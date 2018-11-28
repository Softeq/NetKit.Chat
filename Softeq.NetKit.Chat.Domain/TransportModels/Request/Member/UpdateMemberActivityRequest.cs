// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Member
{
    public class UpdateMemberActivityRequest
    {
        public UpdateMemberActivityRequest(string saasUserId, string connectionId, string userAgent)
        {
            SaasUserId = saasUserId;
            ConnectionId = connectionId;
            UserAgent = userAgent;
        }

        public string SaasUserId { get; }

        public string ConnectionId { get; }

        public string UserAgent { get; }
    }
}