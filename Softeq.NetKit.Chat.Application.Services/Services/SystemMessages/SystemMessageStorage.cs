// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;

namespace Softeq.NetKit.Chat.Application.Services.Services.SystemMessages
{
    public static class SystemMessageStorage
    {
        public static List<SystemMessageWithKey> ChannelModifyMessageStorage = new List<SystemMessageWithKey>
        {
            new SystemMessageWithKey {Key = SystemMessagesKey.ChannelLeft, Message = "left channel"},
            new SystemMessageWithKey {Key = SystemMessagesKey.ChannelJoined, Message = "joined channel"},
            new SystemMessageWithKey {Key = SystemMessagesKey.ChannelUpdated, Message = "updated channel"},
            new SystemMessageWithKey {Key = SystemMessagesKey.ChannelClosed, Message = "closed channel"}
        };
    }
}
