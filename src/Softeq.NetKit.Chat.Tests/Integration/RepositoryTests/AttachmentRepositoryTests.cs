// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.RepositoryTests
{
    public class AttachmentRepositoryTests : BaseTest
    {
        private readonly Message _message;

        public AttachmentRepositoryTests()
        {
            var memberId = new Guid("FE728AF3-DDE7-4B11-BB9B-55C3862262AA");
            var channelId = new Guid("FE728AF3-DDE7-4B11-BB9B-11C3862262EE");
            var messageId = new Guid("67303234-4653-48fb-8ba6-8719d6770811");

            var member = new Member
            {
                Id = memberId,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            var channel = new Channel
            {
                Id = channelId,
                CreatorId = member.Id,
                Name = "testAttachmentChannel",
                MembersCount = 0,
                Type = ChannelType.Public
            };
            UnitOfWork.ChannelRepository.AddChannelAsync(channel).GetAwaiter().GetResult();

            _message = new Message
            {
                Id = messageId,
                Type = MessageType.Default,
                ChannelId = channelId,
                OwnerId = memberId,
                Owner = member
            };
            UnitOfWork.MessageRepository.AddMessageAsync(_message).GetAwaiter().GetResult();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AddAttachmentAsync_ShouldAddAttachment()
        {
            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                ContentType = "test",
                Created = DateTimeOffset.UtcNow,
                FileName = "test",
                MessageId = _message.Id,
                Size = 100
            };

            await UnitOfWork.AttachmentRepository.AddAttachmentAsync(attachment);

            var newAttachment = await UnitOfWork.AttachmentRepository.GetAttachmentAsync(attachment.Id);

            newAttachment.Should().BeEquivalentTo(attachment);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task DeleteAttachmentAsync_ShouldDeleteAttachment()
        {
            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                ContentType = "test",
                Created = DateTimeOffset.UtcNow,
                FileName = "test",
                MessageId = _message.Id,
                Size = 100
            };

            await UnitOfWork.AttachmentRepository.AddAttachmentAsync(attachment);

            await UnitOfWork.AttachmentRepository.DeleteAttachmentAsync(attachment.Id);

            var newAttachment = await UnitOfWork.AttachmentRepository.GetAttachmentAsync(attachment.Id);

            newAttachment.Should().BeNull();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetAttachmentByIdAsync_ShouldReturnExistingAttachment()
        {
            var attachment = new Attachment
            {
                Id = Guid.NewGuid(),
                ContentType = "test",
                Created = DateTimeOffset.UtcNow,
                FileName = "test",
                MessageId = _message.Id,
                Size = 100
            };

            await UnitOfWork.AttachmentRepository.AddAttachmentAsync(attachment);

            var newAttachment = await UnitOfWork.AttachmentRepository.GetAttachmentAsync(attachment.Id);

            newAttachment.Should().BeEquivalentTo(attachment);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetMessageAttachmentsAsync_ShouldReturnAllMessageAttachments()
        {
            var attachments = new List<Attachment> {
                new Attachment
                {
                    Id = new Guid("14D00154-F363-4E83-8F4B-4B9EE303D7D2"),
                    ContentType = "jpg",
                    Created = DateTimeOffset.UtcNow,
                    FileName = "a1.jpg",
                    MessageId = _message.Id,
                    Size = 100
                },
                new Attachment
                {
                    Id = new Guid("BB3E9697-904D-4821-914E-CCE1BD14EF5C"),
                    ContentType = "jpg",
                    Created = DateTimeOffset.UtcNow,
                    FileName = "a2.jpg",
                    MessageId = _message.Id,
                    Size = 222
                }
            };

            foreach (var attachment in attachments)
            {
                await UnitOfWork.AttachmentRepository.AddAttachmentAsync(attachment);
            }

            var newAttachments = await UnitOfWork.AttachmentRepository.GetMessageAttachmentsAsync(_message.Id);

            newAttachments.Should().BeEquivalentTo(attachments);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetMessageAttachmentsCountAsync_ShouldReturnCorrectCount()
        {
            var attachments = new[]
            {
                new Attachment
                {
                    Id = Guid.NewGuid(),
                    ContentType = "jpg",
                    Created = DateTimeOffset.UtcNow,
                    FileName = "pic",
                    MessageId = _message.Id,
                    Size = 100
                },
                new Attachment
                {
                    Id = Guid.NewGuid(),
                    ContentType = "png",
                    Created = DateTimeOffset.UtcNow,
                    FileName = "image",
                    MessageId = _message.Id,
                    Size = 100
                }
            };

            foreach (var attachment in attachments)
            {
                await UnitOfWork.AttachmentRepository.AddAttachmentAsync(attachment);
            }

            var attachmentsCount = await UnitOfWork.AttachmentRepository.GetMessageAttachmentsCountAsync(_message.Id);

            attachmentsCount.Should().Be(attachments.Length);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task DeleteMessageAttachmentsAsync_ShouldDeleteAllMessageAttachments()
        {
            var attachments = new[]
            {
                new Attachment
                {
                    Id = Guid.NewGuid(),
                    ContentType = "jpg",
                    Created = DateTimeOffset.UtcNow,
                    FileName = "pic",
                    MessageId = _message.Id,
                    Size = 100
                },
                new Attachment
                {
                    Id = Guid.NewGuid(),
                    ContentType = "png",
                    Created = DateTimeOffset.UtcNow,
                    FileName = "image",
                    MessageId = _message.Id,
                    Size = 100
                }
            };

            foreach (var attachment in attachments)
            {
                await UnitOfWork.AttachmentRepository.AddAttachmentAsync(attachment);
            }

            await UnitOfWork.AttachmentRepository.DeleteMessageAttachmentsAsync(_message.Id);

            var attachmentsCount = await UnitOfWork.AttachmentRepository.GetMessageAttachmentsCountAsync(_message.Id);

            attachmentsCount.Should().Be(0);
        }
    }
}