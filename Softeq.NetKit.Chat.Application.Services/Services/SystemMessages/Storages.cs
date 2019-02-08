// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;

namespace Softeq.NetKit.Chat.Application.Services.Services.SystemMessages
{
    public static class Storages
    {
        public static List<SystemMessageWithKey> SystemMessageStorage = new List<SystemMessageWithKey>
        {
            new SystemMessageWithKey {Key = "ChannelLeft", Message = "left channel"},
            new SystemMessageWithKey {Key = "ChannelJoined", Message = "joined channel"},
            new SystemMessageWithKey {Key = "ChannelUpdated", Message = "updated channel"},
            new SystemMessageWithKey {Key = "ChannelClosed", Message = "closed channel"}
        };
    }
}
