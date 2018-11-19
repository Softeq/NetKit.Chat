// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Message.TransportModels.Response;

namespace Softeq.NetKit.Chat.Domain.Channel.TransportModels.Response
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