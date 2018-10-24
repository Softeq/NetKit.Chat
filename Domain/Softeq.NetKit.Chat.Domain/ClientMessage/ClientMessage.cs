// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.Base;

namespace Softeq.NetKit.Chat.Domain.ClientMessage
{
    public class ClientMessage : IBaseEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Channel { get; set; }
    }
}