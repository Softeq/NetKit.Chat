using System;
using Newtonsoft.Json;
using Softeq.NetKit.Chat.Client.SDK.Enums;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Message;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Response.Member
{
    public class ConversationResponse
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public int UnreadMessagesCount { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsClosed { get; set; }
        public bool IsMuted { get; set; }
        public bool IsPinned { get; set; }
        [JsonIgnore]
        public bool IsCreatedByMe { get; set; }
        public ChannelType Type { get; set; }
        public MessageResponse LastMessage { get; set; }
    }
}