// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Linq;

namespace Softeq.NetKit.Chat.Application.Services.Services.SystemMessages
{
    public class ChatSystemMessagesService : IChatSystemMessagesService
    {
        public string FormatSystemMessage(SystemMessagesKey key, string memberName, string channelName)
        {
            return $"{memberName} {SystemMessageStorage.ChannelModifyStorage.FirstOrDefault(x => x.Key == key)?.Message} {channelName}";
        }
    }
}
