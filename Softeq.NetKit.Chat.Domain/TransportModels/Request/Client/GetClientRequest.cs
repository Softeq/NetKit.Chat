// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Client
{
    public class GetClientRequest : UserRequest
    {
        public GetClientRequest(string saasUserId, string clientConnectionId) 
            : base(saasUserId)
        {
            ClientConnectionId = clientConnectionId;
        }

        public string ClientConnectionId { get; }
    }
}