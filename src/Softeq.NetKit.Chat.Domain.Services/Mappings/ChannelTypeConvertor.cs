// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Domain.Services.Mappings
{
   public static class ChannelTypeConvertor
    {
        public static ChannelType Convert(Client.SDK.Enums.ChannelType type)
        {
            return (ChannelType) type;
        }
    }
}
