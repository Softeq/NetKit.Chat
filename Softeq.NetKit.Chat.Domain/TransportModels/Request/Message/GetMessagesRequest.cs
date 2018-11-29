// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Message
{
    public class GetMessagesRequest : UserRequest
    {
        public GetMessagesRequest(string saasUserId, Guid channelId, Guid messageId, DateTimeOffset messageCreatedDate, int? pageSize)
            : base(saasUserId)
        {
            ChannelId = channelId;
            MessageId = messageId;
            MessageCreatedDate = messageCreatedDate;
            PageSize = pageSize;
        }

        public Guid ChannelId { get; }

        public Guid MessageId { get; }

        public DateTimeOffset MessageCreatedDate { get; }

        public int? PageSize { get; }
    }
}