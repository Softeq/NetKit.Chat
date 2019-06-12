// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.DomainModels.Constants;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Localization;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Message;
using Softeq.NetKit.Chat.TransportModels.Models.Visitors.Localization;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Visitors.Localization
{
    public class ChannelNameChangedLocalizationVisitor : ILocalizationVisitor<MessageResponse>
    {
        private readonly ChannelSummaryResponse _channel;

        public ChannelNameChangedLocalizationVisitor(ChannelSummaryResponse channel)
        {
            _channel = channel;
        }

        public void Visit(MessageResponse entity)
        {
            entity.Localization = new LocalizationResponse
            {
                Key = LocalizationKeys.SystemChannelNameChanged,
                Parameters = new Dictionary<string, string>
                {
                    [nameof(_channel.Name)] = _channel.Name
                }
            };
        }
    }
}
