// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Response.Member
{
    public class MemberSummaryResponse
    {
        public Guid Id { get; set; }
        //TODO rework model to avoid using ignore attribute
        [JsonIgnore]
        public string SaasUserId { get; set; }
        public string UserName { get; set; }
        public UserRole Role { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public UserStatus Status { get; set; }

        public bool IsActive { get; set; }
        public DateTimeOffset LastActivity { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
    }
}
