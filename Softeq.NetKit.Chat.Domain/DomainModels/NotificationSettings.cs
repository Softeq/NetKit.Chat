using System;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
    public class NotificationSettings : IBaseEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid MemberId { get; set; }
        public NotificationSettingValue IsChannelNotificationsDisabled { get; set; } = NotificationSettingValue.Enabled;
    }
}
