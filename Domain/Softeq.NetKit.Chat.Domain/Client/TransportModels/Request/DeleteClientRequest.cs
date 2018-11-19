// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.Client.TransportModels.Request
{
    public class DeleteClientRequest
    {
        public DeleteClientRequest(string clientConnectionId, String saasUserId)
        {
            ClientConnectionId = clientConnectionId;
            SaasUserId = saasUserId;
        }

        public string ClientConnectionId { get; }
        public string SaasUserId { get; set; }
    }
}