// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Domain.Services.Converters
{
    public static class MessageTypeConverter
    {
        public static MessageType Convert(Chat.TransportModels.Enums.MessageType messageType)
        {
            switch (messageType)
            {
                case Chat.TransportModels.Enums.MessageType.Default:
                    return MessageType.Default;
                case Chat.TransportModels.Enums.MessageType.Forward:
                    return MessageType.Forward;
                default: throw new ArgumentOutOfRangeException($"Message type {messageType} not found");
            }
        }
    }
}
