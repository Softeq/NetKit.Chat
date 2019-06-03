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
        //TODO rework model to avoid using ignore attribute
        [JsonIgnore]
        public string SaasUserId { get; }
    }
}