// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.MessageAttachment;

namespace Softeq.NetKit.Chat.SignalR.Sockets
{
    public interface IMessageSocketService
    {
        Task<MessageResponse> AddMessageAsync(CreateMessageRequest request);
        Task DeleteMessageAsync(DeleteMessageRequest request);
        Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest request);
        Task<AttachmentResponse> AddMessageAttachmentAsync(AddMessageAttachmentRequest request);
        Task DeleteMessageAttachmentAsync(DeleteMessageAttachmentRequest request);
        Task SetLastReadMessageAsync(SetLastReadMessageRequest request);
    }
}