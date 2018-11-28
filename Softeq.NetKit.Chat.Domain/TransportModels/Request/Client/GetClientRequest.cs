// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Client
{
    public class GetClientRequest
    {
        public GetClientRequest(string saasUserId, string clientConnectionId)
        {
            SaasUserId = saasUserId;
            ClientConnectionId = clientConnectionId;
        }

        public string SaasUserId { get; }

        public string ClientConnectionId { get; }
    }
}