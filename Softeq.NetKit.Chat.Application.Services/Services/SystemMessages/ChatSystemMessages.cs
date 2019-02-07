// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.DomainModels;
using System.Linq;

namespace Softeq.NetKit.Chat.Application.Services.Services.SystemMessages
{
    public class ChatSystemMessages : IChatSystemMessages
    {
        public string FormatSystemMessage(string key, Member member, Channel channel)
        {
            return $"{member.Name} {Constants.SystemMessageStorage.FirstOrDefault(x => x.Key == key)} {channel.Name}";
        }
    }
}
