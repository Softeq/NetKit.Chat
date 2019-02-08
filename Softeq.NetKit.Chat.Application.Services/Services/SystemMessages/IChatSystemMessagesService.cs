// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Application.Services.Services.SystemMessages
{
    public interface IChatSystemMessagesService
    {
        string FormatSystemMessage(string key, string memberName, string channelName);
    }
}
