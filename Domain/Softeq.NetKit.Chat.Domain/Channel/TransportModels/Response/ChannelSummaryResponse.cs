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

        // TODO: Implement logic for uploading photo
        public string PhotoUrl
        {
            get
            {
                var avatarUrls = new List<string>()
                {
                    "https://f4.bcbits.com/img/0008335057_10.jpg",
                    "https://cdn.pixabay.com/photo/2015/10/23/17/03/eye-1003315_960_720.jpg",
                    "https://images.homedepot-static.com/productImages/a4e315b5-e2b7-4fd9-94a0-c7d51408285d/svn/brady-stock-signs-94143-64_1000.jpg"
                };
                var avatarCode = Math.Abs(Name.GetHashCode() % avatarUrls.Count);
                return avatarUrls[avatarCode];
            }
        }
    }
}