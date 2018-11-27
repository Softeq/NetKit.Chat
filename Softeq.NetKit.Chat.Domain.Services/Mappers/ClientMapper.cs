// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;

namespace Softeq.NetKit.Chat.Domain.Services.Mappers
{
    internal static class ClientMapper
    {
        public static ClientResponse ToClientResponse(this DomainModels.Client client)
        {
            var clientResponse = new ClientResponse();
            if (client != null)
            {
                clientResponse.Id = client.Id;
                clientResponse.ConnectionClientId = client.ClientConnectionId;
                clientResponse.SaasUserId = client.Member.SaasUserId;
                clientResponse.UserName = client.Name;
            }
            return clientResponse;
        }
    }
}