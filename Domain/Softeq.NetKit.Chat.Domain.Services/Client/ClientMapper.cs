// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.Client.TransportModels.Response;

namespace Softeq.NetKit.Chat.Domain.Services.Client
{
    internal static class ClientMapper
    {
        public static ConnectionResponse ToClientResponse(this Domain.Client.Connection client, string saasUserId)
        {
            var clientResponse = new ConnectionResponse();
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