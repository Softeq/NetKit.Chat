// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;

namespace Softeq.NetKit.Chat.Domain.Message.TransportModels.Request
{
    public class GetMessagesRequest : UserRequest
    {
        public GetMessagesRequest(string userId, Guid channelId, Guid messageId, DateTimeOffset messageCreatedDate,
            int? pageSize) : base(userId)
        {
            ChannelId = channelId;
            MessageId = messageId;
            MessageCreatedDate = messageCreatedDate;
            PageSize = pageSize;
        }

        public Guid ChannelId { get; set; }
        public Guid MessageId { get; set; }
        public DateTimeOffset MessageCreatedDate { get; set; }
        public int? PageSize { get; set; }
    }
}