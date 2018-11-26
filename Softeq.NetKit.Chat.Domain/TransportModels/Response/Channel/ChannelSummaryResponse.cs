// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Newtonsoft.Json;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel
{
    public class ChannelSummaryResponse
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public int UnreadMessagesCount { get; set; }
        public string Name { get; set; }
        public bool IsClosed { get; set; }
        public bool IsMuted { get; set; }
        public bool IsPinned { get; set; }
        [JsonIgnore]
        public Guid? CreatorId { get; set; }

        public MemberSummary Creator { get; set; }
        public string CreatorSaasUserId { get; set; }
        public string Description { get; set; }
        public string WelcomeMessage { get; set; }
        public ChannelType Type { get; set; }
        public MessageResponse LastMessage { get; set; }
        public string PhotoUrl { get; set; }
    }
}