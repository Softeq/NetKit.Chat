// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Infrastructure.Storage.Sql.Repositories
{
    public interface IAttachmentRepository
    {
        Task AddAttachmentAsync(Attachment attachment);
        Task DeleteAttachmentAsync(Guid attachmentId);
        Task<Attachment> GetAttachmentByIdAsync(Guid attachmentId);
        Task<List<Attachment>> GetMessageAttachmentsAsync(Guid messageId);
        Task DeleteMessageAttachmentsAsync(Guid messageId);
    }
}