// Developed by Softeq Development Corporation
// http://www.softeq.com

using Newtonsoft.Json;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request
{
    public class UserRequest
    {
        public UserRequest(string saasUserId)
        {
            SaasUserId = saasUserId;
        }
        [JsonIgnore]
        public string SaasUserId { get; }
    }
}