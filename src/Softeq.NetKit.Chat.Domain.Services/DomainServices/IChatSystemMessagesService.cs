// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IChatSystemMessagesService
    {
        string FormatSystemMessage(SystemMessagesKey key, string memberName, string channelName);
    }
}
