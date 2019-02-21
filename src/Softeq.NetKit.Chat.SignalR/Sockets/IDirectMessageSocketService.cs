// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;

namespace Softeq.NetKit.Chat.SignalR.Sockets
{
    public interface IDirectMessageSocketService
    {
        Task<DirectMessageResponse> AddMessageAsync(CreateDirectMessageRequest request);
        Task<DirectMessageResponse> DeleteMessageAsync(string saasUserId, Guid messageId, Guid directChannelId);
        Task<DirectMessageResponse> UpdateMessageAsync(UpdateDirectMessageRequest request);
    }
}
