// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Message;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.MessageAttachment;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IMessageService
    {
        Task<MessageResponse> CreateMessageAsync(CreateMessageRequest request);
        Task ArchiveMessageAsync(ArchiveMessageRequest request);
        Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest request);
        Task<MessageResponse> GetMessageByIdAsync(Guid messageId);
        Task<AttachmentResponse> AddMessageAttachmentAsync(AddMessageAttachmentRequest request);
        Task DeleteMessageAttachmentAsync(DeleteMessageAttachmentRequest request);
        Task SetLastReadMessageAsync(SetLastReadMessageRequest request);
        Task<MessagesResult> GetOlderMessagesAsync(GetMessagesRequest request);
        Task<MessagesResult> GetMessagesAsync(GetMessagesRequest request);
        Task<MessagesResult> GetLastMessagesAsync(GetLastMessagesRequest request);
        Task<IReadOnlyCollection<Guid>> FindMessageIdsAsync(Guid channelId, string searchText);
        Task<MessageResponse> CreateSystemMessageAsync(CreateMessageRequest request);
    }
}