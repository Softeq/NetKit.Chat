// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Data.Persistent.Sql.Database;
using Softeq.NetKit.Chat.Domain.DomainModels;

namespace Softeq.NetKit.Chat.Data.Persistent.Sql.Repositories
{
    internal class AttachmentRepository : IAttachmentRepository
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public AttachmentRepository(ISqlConnectionFactory sqlConnectionFactory)
        {
            Ensure.That(sqlConnectionFactory).IsNotNull();

            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task AddAttachmentAsync(Attachment attachment)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"INSERT INTO Attachments(Id, ContentType, Created, FileName, MessageId, Size)
                                 VALUES (@Id, @ContentType, @Created, @FileName, @MessageId, @Size)";

                await connection.ExecuteScalarAsync(sqlQuery, attachment);
            }
        }

        public async Task DeleteAttachmentAsync(Guid attachmentId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"DELETE FROM Attachments 
                                 WHERE Id = @attachmentId";

                await connection.ExecuteScalarAsync<Attachment>(sqlQuery, new { attachmentId });
            }
        }

        public async Task<Attachment> GetAttachmentAsync(Guid attachmentId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM Attachments
                                 WHERE Id = @attachmentId";

                return (await connection.QueryAsync<Attachment>(sqlQuery, new { attachmentId })).FirstOrDefault();
            }
        }

        public async Task<IReadOnlyCollection<Attachment>> GetMessageAttachmentsAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT *
                                 FROM Attachments
                                 WHERE MessageId = @messageId";

                return (await connection.QueryAsync<Attachment>(sqlQuery, new { messageId })).ToList().AsReadOnly();
            }
        }

        public async Task<int> GetMessageAttachmentsCountAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"SELECT COUNT(*)
                                 FROM Attachments
                                 WHERE MessageId = @messageId";

                return await connection.ExecuteScalarAsync<int>(sqlQuery, new { messageId });
            }
        }

        public async Task DeleteMessageAttachmentsAsync(Guid messageId)
        {
            using (var connection = _sqlConnectionFactory.CreateConnection())
            {
                var sqlQuery = @"DELETE FROM Attachments 
                                 WHERE MessageId = @messageId";

                await connection.ExecuteScalarAsync<Attachment>(sqlQuery, new { messageId });
            }
        }
    }
}