// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System;
using System.Collections.Generic;
using Softeq.NetKit.Chat.Domain.Base;

namespace Softeq.NetKit.Chat.Domain.Message
{
    public class Message : IBaseEntity<Guid>, ICreated
    {
        public Guid ChannelId { get; set; }
        public Guid? OwnerId { get; set; }
        public Channel.Channel Channel { get; set; }
        public Member.Member Owner { get; set; }
        public string Body { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public MessageType Type { get; set; }

        // Notifications
        public string ImageUrl { get; set; }
        public List<Notification.Notification> Notifications { get; set; }
        public List<Attachment.Attachment> Attachments { get; set; }
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}