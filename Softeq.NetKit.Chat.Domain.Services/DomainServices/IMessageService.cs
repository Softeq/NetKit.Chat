﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.MessageAttachment;
using Softeq.QueryUtils;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
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
        Task<bool> IsAttachmentLimitExceededAsync(Guid messageId);
        Task SetLastReadMessageAsync(SetLastReadMessageRequest request);
        Task<MessagesResult> GetOlderMessagesAsync(GetMessagesRequest request);
        Task<MessagesResult> GetMessagesAsync(GetMessagesRequest request);
        Task<MessagesResult> GetLastMessagesAsync(GetLastMessagesRequest request);
        Task<IReadOnlyCollection<Guid>> FindMessageIdsAsync(Guid channelId, string searchText);
    }
}