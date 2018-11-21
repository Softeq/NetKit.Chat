// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Attachment.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Message.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Message.TransportModels.Response;
using Softeq.QueryUtils;

namespace Softeq.NetKit.Chat.Domain.Message
{
    public interface IMessageService
    {
        Task<MessageResponse> CreateMessageAsync(CreateMessageRequest request);
        Task DeleteMessageAsync(DeleteMessageRequest request);
        Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest request);
        Task<MessageResponse> GetMessageByIdAsync(Guid messageId);
        Task<AttachmentResponse> AddMessageAttachmentAsync(AddMessageAttachmentRequest request);
        Task DeleteMessageAttachmentAsync(DeleteMessageAttachmentRequest request);
        Task<PagedResults<MessageResponse>> GetChannelMessagesAsync(MessageRequest request);
        Task<int> GetMessageAttachmentsCount(Guid messageId);
        Task SetLastReadMessageAsync(SetLastReadMessageRequest request);
        Task<MessagesResult> GetOlderMessagesAsync(GetMessagesRequest request);
        Task<MessagesResult> GetMessagesAsync(GetMessagesRequest request);
        Task<MessagesResult> GetLastMessagesAsync(GetLastMessagesRequest request);
    }
}