// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Repositories
{
    internal class AttachmentRepository : BaseRepository, IAttachmentRepository
    {
        public AttachmentRepository(ISqlConnectionFactory sqlConnectionFactory) : base(sqlConnectionFactory)
        {
        }

        public async Task AddAttachmentAsync(Attachment attachment)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    INSERT INTO Attachments
                    (
                        {nameof(Attachment.Id)}, 
                        {nameof(Attachment.ContentType)}, 
                        {nameof(Attachment.Created)}, 
                        {nameof(Attachment.FileName)}, 
                        {nameof(Attachment.MessageId)}, 
                        {nameof(Attachment.Size)}
                    ) VALUES 
                    (
                        @{nameof(Attachment.Id)}, 
                        @{nameof(Attachment.ContentType)}, 
                        @{nameof(Attachment.Created)}, 
                        @{nameof(Attachment.FileName)}, 
                        @{nameof(Attachment.MessageId)}, 
                        @{nameof(Attachment.Size)}
                    )";

                await connection.ExecuteScalarAsync(sqlQuery, attachment);
            }
        }

        public async Task DeleteAttachmentAsync(Guid attachmentId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    DELETE 
                    FROM 
                        Attachments 
                    WHERE 
                        {nameof(Attachment.Id)} = @{nameof(attachmentId)}";

                await connection.ExecuteScalarAsync<Attachment>(sqlQuery, new { attachmentId });
            }
        }

        public async Task<Attachment> GetAttachmentAsync(Guid attachmentId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT
                        {nameof(Attachment.Id)},
                        {nameof(Attachment.ContentType)},
                        {nameof(Attachment.Created)},
                        {nameof(Attachment.FileName)},
                        {nameof(Attachment.MessageId)},
                        {nameof(Attachment.Size)}
                    FROM
                        Attachments
                    WHERE 
                        {nameof(Attachment.Id)} = @{nameof(attachmentId)}";

                return (await connection.QueryAsync<Attachment>(sqlQuery, new { attachmentId })).FirstOrDefault();
            }
        }

        public async Task<IReadOnlyCollection<Attachment>> GetMessageAttachmentsAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT
                        {nameof(Attachment.Id)},
                        {nameof(Attachment.ContentType)},
                        {nameof(Attachment.Created)},
                        {nameof(Attachment.FileName)},
                        {nameof(Attachment.MessageId)},
                        {nameof(Attachment.Size)}
                    FROM 
                        Attachments
                    WHERE 
                        {nameof(Attachment.MessageId)} = @{nameof(messageId)}";

                return (await connection.QueryAsync<Attachment>(sqlQuery, new { messageId })).ToList().AsReadOnly();
            }
        }

        public async Task<int> GetMessageAttachmentsCountAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    SELECT 
                        COUNT(*)
                    FROM 
                        Attachments
                    WHERE 
                        {nameof(Attachment.MessageId)} = @{nameof(messageId)}";

                return await connection.ExecuteScalarAsync<int>(sqlQuery, new { messageId });
            }
        }

        public async Task DeleteMessageAttachmentsAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = $@"
                    DELETE 
                    FROM 
                        Attachments
                    WHERE 
                        {nameof(Attachment.MessageId)} = @{nameof(messageId)}";

                await connection.ExecuteScalarAsync<Attachment>(sqlQuery, new { messageId });
            }
        }
    }
}