// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.DomainModels;
using System;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Response.SystemMessage
{
    public class SystemMessageResponse
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        public MessageType Type { get; set; }
    }
}
