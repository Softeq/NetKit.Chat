// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;

namespace Softeq.NetKit.Chat.Domain.Services.Client
{
    internal static class ClientMapper
    {
        public static ClientResponse ToClientResponse(this DomainModels.Client client, string saasUserId)
        {
            var clientResponse = new ClientResponse();
            if (client != null)
            {
                clientResponse.Id = client.Id;
                clientResponse.ConnectionClientId = client.ClientConnectionId;
                clientResponse.SaasUserId = saasUserId;
                clientResponse.UserName = client.Name;
            }
            return clientResponse;
        }
    }
}