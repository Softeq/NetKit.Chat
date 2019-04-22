// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Chat.Domain.DomainModels.Constants;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Localization;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Visitors.Localization
{
    public class ChannelIconChangedLocalizationVisitor : ILocalizationVisitor<MessageResponse>
    {
        private readonly ChannelSummaryResponse _channel;

        public ChannelIconChangedLocalizationVisitor(ChannelSummaryResponse channel)
        {
            _channel = channel;
        }

        public void Visit(MessageResponse entity)
        {
            entity.Localization = new LocalizationResponse
            {
                Key = LocalizationKeys.SystemChannelIconChanged,
                Parameters = new Dictionary<string, string>
                {
                    [nameof(_channel.PhotoUrl)] = _channel.PhotoUrl
                }
            };
        }
    }
}
