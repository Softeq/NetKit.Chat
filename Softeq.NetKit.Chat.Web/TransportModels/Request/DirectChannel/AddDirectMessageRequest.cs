// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Web.TransportModels.Request.DirectChannel
{
    public class AddDirectMessageRequest
    {
        public AddDirectMessageRequest(string body, Guid directChannelId)
        {
            Body = body;
            DirectChannelId = directChannelId;
        }

        public string Body { get; set; }
        public Guid DirectChannelId { get; set; }
    }
}
