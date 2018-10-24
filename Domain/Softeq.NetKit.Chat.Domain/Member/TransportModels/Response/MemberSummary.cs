// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Newtonsoft.Json;

namespace Softeq.NetKit.Chat.Domain.Member.TransportModels.Response
{
    public class MemberSummary
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string SaasUserId { get; set; }
        public string UserName { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public bool IsAfk { get; set; }
        public DateTimeOffset LastActivity { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
    }
}