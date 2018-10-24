// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Softeq.NetKit.Chat.Domain.Base;
using Softeq.NetKit.Chat.Domain.ChannelMember;

namespace Softeq.NetKit.Chat.Domain.Member
{
    public class Member : IBaseEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset LastActivity { get; set; }
        public DateTimeOffset? LastNudged { get; set; }
        public UserStatus Status { get; set; }

        public bool IsAfk { get; set; }
        [StringLength(255)]

        public string SaasUserId { get; set; }

        public string Email { get; set; }

        public UserRole Role { get; set; }
        public bool IsBanned { get; set; }

        public string PhotoName { get; set; }
        public List<Channel.Channel> OwnedChannels { get; set; }

        public List<Message.Message> Messages { get; set; }

        // List of clients that are currently connected for this user
        public List<Client.Client> ConnectedClients { get; set; }
        public List<ChannelMembers> Channels { get; set; }

        public List<Notification.Notification> Notifications { get; set; }
    }
}