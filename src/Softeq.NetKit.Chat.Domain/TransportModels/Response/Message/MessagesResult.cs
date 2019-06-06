// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Message;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Response.Message
{
    public class MessagesResult
    {
        public int? PageSize { get; set; }
        public IEnumerable<MessageResponse> Results { get; set; }
    }
}