// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.QueryUtils;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Request.Message
{
    public class MessageRequest : UserRequest, IPagedQuery
    {
        public MessageRequest(string saasUserId, Guid channelId, int page, int pageSize) 
            : base(saasUserId)
        {
            ChannelId = channelId;
            Page = page;
            PageSize = pageSize;
        }

        public Guid ChannelId { get; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}