// Developed by Softeq Development Corporation
// http://www.softeq.com


using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.SystemMessage;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface ISystemMessageService
    {
        Task<SystemMessageResponse> CreateMessageAsync(Guid channelId, string body);
    }
}
