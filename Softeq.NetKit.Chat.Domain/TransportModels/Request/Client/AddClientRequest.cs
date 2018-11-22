// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Client
{
    public class AddClientRequest
    {
        public string SaasUserId { get; set; }
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
        public string UserAgent { get; set; }
    }
}