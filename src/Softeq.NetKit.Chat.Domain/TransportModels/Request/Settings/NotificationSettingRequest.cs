using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Settings
{
    public class NotificationSettingRequest : UserRequest
    {
        public NotificationSettingRequest(string userId) : base(userId)
        {
        }

        public NotificationSettingKey Key { get; set; }
        public NotificationSettingValue Value { get; set; }
    }
}
