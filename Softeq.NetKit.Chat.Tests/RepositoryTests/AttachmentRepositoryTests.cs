// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Tests.Abstract;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.RepositoryTests
{
    public class AttachmentRepositoryTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("FE728AF3-DDE7-4B11-BB9B-55C3862262AA");
        private readonly Guid _channelId = new Guid("FE728AF3-DDE7-4B11-BB9B-11C3862262EE");
        private readonly Guid _messageId = new Guid("67303234-4653-48fb-8ba6-8719d6770811");

        public AttachmentRepositoryTests()
        {
            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTime.UtcNow,
                Status = UserStatus.Active
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            var channel = new Channel
            {
                Id = _channelId,
                CreatorId = member.Id,
                Name = "testAttachmentChannel",
                MembersCount = 0,
                Type = ChannelType.Public
            };
            UnitOfWork.ChannelRepository.AddChannelAsync(channel).GetAwaiter().GetResult();

            var message = new Message
            {
                Id = _messageId,
                Type = MessageType.Default,
                ChannelId = _channelId
            };
            UnitOfWork.MessageRepository.AddMessageAsync(message).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task AddAttachmentAsyncTest()
        {
            // Arrange
            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                ContentType = "test",
                Created = DateTime.UtcNow,
                FileName = "test",
                MessageId = _messageId,
                Size = 100
            };
            
            // Act
            await UnitOfWork.AttachmentRepository.AddAttachmentAsync(attachment);
            var newAttachment = await UnitOfWork.AttachmentRepository.GetAttachmentByIdAsync(attachment.Id);

            // Assert
            Assert.NotNull(newAttachment);
            Assert.Equal(attachment.Id, newAttachment.Id);
            Assert.Equal(attachment.ContentType, newAttachment.ContentType);
            Assert.Equal(attachment.FileName, newAttachment.FileName);
            Assert.Equal(attachment.MessageId, newAttachment.MessageId);
            Assert.Equal(attachment.Size, newAttachment.Size);
        }

        [Fact]
        public async Task DeleteAttachmentAsyncTest()
        {
            // Arrange
            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                ContentType = "test",
                Created = DateTime.UtcNow,
                FileName = "test",
                MessageId = _messageId,
                Size = 100
            };
            
            // Act
            await UnitOfWork.AttachmentRepository.AddAttachmentAsync(attachment);
            await UnitOfWork.AttachmentRepository.DeleteAttachmentAsync(attachment.Id);
            var newAttachment = await UnitOfWork.AttachmentRepository.GetAttachmentByIdAsync(attachment.Id);

            // Assert
            Assert.Null(newAttachment);
        }

        [Fact]
        public async Task GetAttachmentByIdAsyncTest()
        {
            // Arrange
            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                ContentType = "test",
                Created = DateTime.UtcNow,
                FileName = "test",
                MessageId = _messageId,
                Size = 100
            };

            // Act
            await UnitOfWork.AttachmentRepository.AddAttachmentAsync(attachment);
            var newAttachment = await UnitOfWork.AttachmentRepository.GetAttachmentByIdAsync(attachment.Id);

            // Assert
            Assert.NotNull(newAttachment);
            Assert.Equal(attachment.Id, newAttachment.Id);
            Assert.Equal(attachment.ContentType, newAttachment.ContentType);
            Assert.Equal(attachment.FileName, newAttachment.FileName);
            Assert.Equal(attachment.MessageId, newAttachment.MessageId);
            Assert.Equal(attachment.Size, newAttachment.Size);
        }

        [Fact]
        public async Task GetMessageAttachmentsCountAsync_ShouldReturnCorrectCount()
        {
            var attachments = new[]
            {
                new Attachment
                {
                    Id = Guid.NewGuid(),
                    ContentType = "jpg",
                    Created = DateTime.UtcNow,
                    FileName = "pic",
                    MessageId = _messageId,
                    Size = 100
                },
                new Attachment
                {
                    Id = Guid.NewGuid(),
                    ContentType = "png",
                    Created = DateTime.UtcNow,
                    FileName = "image",
                    MessageId = _messageId,
                    Size = 100
                }
            };

            foreach (var attachment in attachments)
            {
                await UnitOfWork.AttachmentRepository.AddAttachmentAsync(attachment);
            }

            var count = await UnitOfWork.AttachmentRepository.GetMessageAttachmentsCountAsync(_messageId);

            Assert.Equal(attachments.Length, count);
        }
    }
}