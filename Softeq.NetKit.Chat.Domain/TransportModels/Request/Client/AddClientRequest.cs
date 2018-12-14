// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Client
{
    public class AddClientRequest : UserRequest
    {
        public AddClientRequest(string saasUserId, string userName, string connectionId, string userAgent, string email)
            : base(saasUserId)
        {
            UserName = userName;
            ConnectionId = connectionId;
            UserAgent = userAgent;
            Email = email;
        }

        public string UserName { get; }

        public string ConnectionId { get; }

        public string UserAgent { get; }

        public string Email { get; }
    }
}