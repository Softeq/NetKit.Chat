// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Linq;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public class ChatSystemMessagesService : IChatSystemMessagesService
    {
        public string FormatSystemMessage(SystemMessagesKey key, string memberName, string channelName)
        {
            return $"{memberName} {SystemMessageStorage.ChannelModifyMessageStorage.FirstOrDefault(x => x.Key == key)?.Message} {channelName}";
        }
    }
}
