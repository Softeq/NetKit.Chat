// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Client.SDK.Enums;

namespace Softeq.NetKit.Chat.Domain.Services.Mappings
{
   public static class MessageTypeConvertor
    {
        public static MessageType Convert(Client.SDK.Enums.MessageType type)
        {
            return (MessageType)type;
        }
    }
}
