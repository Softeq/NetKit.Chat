// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Response.Message
{
    public class MessagesResult
    {
        public int? PageSize { get; set; }
        public IEnumerable<MessageResponse> Results { get; set; }
    }
}