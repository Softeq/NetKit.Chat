// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Softeq.NetKit.Chat.Client.SDK.Enums;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
    public class Message : IBaseEntity<Guid>, ICreated
    {
        public Guid Id { get; set; }
        public Guid? ChannelId { get; set; }
        public Guid? OwnerId { get; set; }
        public Channel Channel { get; set; }
        public Member Owner { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public MessageType Type { get; set; }
        public AccessibilityStatus AccessibilityStatus { get; set; }

        // Notifications
        public string ImageUrl { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<Attachment> Attachments { get; set; }

        // Forward message
        public Guid? ForwardMessageId { get; set; }
        public ForwardMessage ForwardedMessage { get; set; }
    }
}
