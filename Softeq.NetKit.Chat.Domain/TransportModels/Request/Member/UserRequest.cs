// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Member
{
    public class UserRequest : BaseRequest
    {
        public UserRequest(string saasUserId)
        {
            SaasUserId = saasUserId;
        }
    }
}