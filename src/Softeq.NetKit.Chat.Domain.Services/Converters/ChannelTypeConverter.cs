// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Domain.Services.Converters
{
   public static class ChannelTypeConverter
    {
        public static ChannelType Convert(Chat.TransportModels.Enums.ChannelType channelType)
        {
            switch (channelType)
            {
                case Chat.TransportModels.Enums.ChannelType.Direct:
                    return ChannelType.Direct;
                case Chat.TransportModels.Enums.ChannelType.Private:
                    return ChannelType.Private;
                case Chat.TransportModels.Enums.ChannelType.Public:
                    return ChannelType.Public;
                default: throw new ArgumentOutOfRangeException($"Channel type {channelType} not found");
            }
        }
    }
}
