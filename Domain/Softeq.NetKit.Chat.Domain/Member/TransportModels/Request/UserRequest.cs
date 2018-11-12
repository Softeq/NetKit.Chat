// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using Softeq.NetKit.Chat.Domain.Base;

namespace Softeq.NetKit.Chat.Domain.Member.TransportModels.Request
{
    public class UserRequest : BaseRequest
    {
        public UserRequest(string saasUserId)
        {
            SaasUserId = saasUserId;
        }
    }
}