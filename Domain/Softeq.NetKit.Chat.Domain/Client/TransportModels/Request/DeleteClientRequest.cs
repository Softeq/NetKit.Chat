// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.Client.TransportModels.Request
{
    public class DeleteClientRequest
    {
        public DeleteClientRequest(string clientConnectionId)
        {
            ClientConnectionId = clientConnectionId;
        }

        public string ClientConnectionId { get; set; }
        public string SaasUserId { get; set; }
    }
}