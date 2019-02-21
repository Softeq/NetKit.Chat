// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.TransportModels.Response.SystemMessage
{
    public class SystemMessageResponse
    {
        public DomainModels.Message Message { get; set; }
        public DomainModels.Member  Member { get; set; }
        public DomainModels.Channel Channel { get; set; }
    }
}
