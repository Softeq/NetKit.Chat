using System;
using System.Collections.Generic;
using System.Text;

namespace Softeq.NetKit.Chat.Domain.Client.TransportModels
{
    public class ClientCache
    {
        public String SaasUserId { get; set; }
        public List<Client> Clients { get; set; }
    }
}
