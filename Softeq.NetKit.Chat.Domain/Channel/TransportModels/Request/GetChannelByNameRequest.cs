// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request
{
    public class GetChannelByNameRequest
    {
        public GetChannelByNameRequest(string channelName)
        {
            ChannelName = channelName;
        }

        public string ChannelName { get; set; }
    }
}