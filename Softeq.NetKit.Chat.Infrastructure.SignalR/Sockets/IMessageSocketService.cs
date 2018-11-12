// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Message.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Message.TransportModels.Response;

namespace Softeq.NetKit.Chat.Infrastructure.SignalR.Sockets
{
    public interface IMessageSocketService
    {
        Task<MessageResponse> AddMessageAsync(CreateMessageRequest createMessageRequest);
        Task DeleteMessageAsync(DeleteMessageRequest request);
        Task UpdateMessageAsync(UpdateMessageRequest request);
        Task AddMessageAttachmentAsync(AddMessageAttachmentRequest request);
        Task DeleteMessageAttachmentAsync(DeleteMessageAttachmentRequest request);
        Task AddLastReadMessageAsync(AddLastReadMessageRequest request);
    }
}