// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Chat.Web.TransportModels.Request.DirectChannel
{
    public class UpdateDirectMessageRequest
    {
        public UpdateDirectMessageRequest(Guid messageId, Guid directChannelId, string body)
        {
            MessageId = messageId;
            DirectChannelId = directChannelId;
            Body = body;
        }

        public Guid MessageId { get; set; }
        public Guid DirectChannelId { get; set; }
        public string Body { get; set; }
    }
}
