// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;

namespace Softeq.NetKit.Chat.SignalR.Sockets
{
    public interface IDirectMessageSocketService
    {
        Task<DirectMessageResponse> AddMessage(CreateDirectMessageRequest request);
        Task<DirectMessageResponse> DeleteMessage(string saasUserId, Guid messageId, Guid directChannelId);
        Task<DirectMessageResponse> UpdateMessage(UpdateDirectMessageRequest request);
    }
}
