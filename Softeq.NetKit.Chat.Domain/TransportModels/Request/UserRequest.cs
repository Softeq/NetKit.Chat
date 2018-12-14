// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request
{
    public class UserRequest
    {
        public UserRequest(string saasUserId)
        {
            SaasUserId = saasUserId;
        }

        public string SaasUserId { get; }
    }
}