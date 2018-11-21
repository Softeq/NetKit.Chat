// Developed by Softeq Development Corporation
// http://www.softeq.com

using Newtonsoft.Json;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request
{
    public class BaseRequest
    {
        public string RequestId { get; set; }

        [JsonIgnore]
        public string SaasUserId { get; set; }
        [JsonIgnore]
        public string ClientConnectionId { get; set; }
    }
}