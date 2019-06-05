// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Localization;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Message;
using Softeq.NetKit.Chat.Domain.DomainModels.Constants;

namespace Softeq.NetKit.Chat.Domain.TransportModels.Visitors.Localization
{
    public class ChannelNameChangedLocalizationVisitor : Client.SDK.Models.Visitors.Localization.ILocalizationVisitor<MessageResponse>
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
