using System;
using System.Collections.Generic;
using System.Text;

namespace Softeq.NetKit.Chat.Domain.Client.TransportModels
{
    public class ConnectionCache
    {
        public String SaasUserId { get; set; }
        public List<Connection> Clients { get; set; }
    }
}
