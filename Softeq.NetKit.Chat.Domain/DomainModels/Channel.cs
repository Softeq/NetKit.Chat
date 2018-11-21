// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Softeq.NetKit.Chat.Domain.DomainModels
{
    public class Channel : IBaseEntity<Guid>, ICreated
    {
        public Guid Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public bool IsClosed { get; set; }
        [StringLength(80)]
        public string Description { get; set; }
        [StringLength(200)]
        public string WelcomeMessage { get; set; }
        public string PhotoUrl { get; set; }

        public ChannelType Type { get; set; }

        // Creator of the channel
        public Guid? CreatorId { get; set; }
        public Member Creator { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        public int MembersCount { get; set; }

        public Settings Settings { get; set; }
        public List<Message> Messages { get; set; }
        public List<ChannelMembers> Members { get; set; }
        public List<Notification> Notifications { get; set; }
    }
}