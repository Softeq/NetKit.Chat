// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Web.TransportModels.Request.Channel
{
    public class UpdateChannelRequest
    {
        public string Name { get; set; }

        public string Topic { get; set; }

        public string WelcomeMessage { get; set; }

        public string PhotoUrl { get; set; }
    }
}