// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Application.Services.Services.SystemMessages
{
    public interface IChatSystemMessages
    {
        string FormatSystemMessage(string key, Member member, Channel channel);
    }
}
