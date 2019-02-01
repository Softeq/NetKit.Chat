// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.MessageAttachment;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MessageServiceTests
{
    public class AddMessageAttachmentAsyncTests : MessageServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfMessageDoesNotExist()
        {
            // Arrange
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Message)null)
                .Verifiable();

            var request = new AddMessageAttachmentRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), null, "jpg", "image/jpeg", 0);

            // Act
            Func<Task> act = async () => { await _messageService.AddMessageAttachmentAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to add message attachment. Message {nameof(request.MessageId)}:{request.MessageId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Message())
                .Verifiable();
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var request = new AddMessageAttachmentRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), null, "jpg", "image/jpeg", 0);

            // Act
            Func<Task> act = async () => { await _messageService.AddMessageAttachmentAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to add message attachment. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfMemberIsNotMessageOwner()
        {
            // Arrange
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Message { OwnerId = new Guid("F19AD922-B0DB-4686-8CB4-F51902800CAE") })
                .Verifiable();
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new Member { Id = new Guid("ABF2CA08-5374-4CED-BE87-6EA93A8B90DA") })
                .Verifiable();

            var request = new AddMessageAttachmentRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), null, "jpg", "image/jpeg", 0);

            // Act
            Func<Task> act = async () => { await _messageService.AddMessageAttachmentAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatAccessForbiddenException>()
                .And.Message.Should().Be($"Unable to add message attachment. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfAttachmentLimitExceeded()
        {
            // Arrange
            var request = new AddMessageAttachmentRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), null, "jpg", "image/jpeg", 0);

            var member = new Member
            {
                Id = new Guid("ABF2CA08-5374-4CED-BE87-6EA93A8B90DA")
            };
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Message { OwnerId = member.Id })
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(member)
                .Verifiable();

            _attachmentRepositoryMock.Setup(x => x.GetMessageAttachmentsCountAsync(It.IsAny<Guid>()))
                .ReturnsAsync(MessageAttachmentsLimit)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _messageService.AddMessageAttachmentAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatInvalidOperationException>()
                .And.Message.Should().Be($"Unable to add message attachment. Attachment limit {MessageAttachmentsLimit} exceeded.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldAddAttachment()
        {
            // Arrange
            var request = new AddMessageAttachmentRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), null, "jpg", "image/jpeg", 0);

            var member = new Member
            {
                Id = new Guid("ABF2CA08-5374-4CED-BE87-6EA93A8B90DA")
            };
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Message { Id = request.MessageId, OwnerId = member.Id })
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(member)
                .Verifiable();

            _attachmentRepositoryMock.Setup(x => x.GetMessageAttachmentsCountAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .ReturnsAsync(MessageAttachmentsLimit - 1)
                .Verifiable();

            _attachmentRepositoryMock.Setup(x => x.AddAttachmentAsync(It.IsAny<Attachment>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            string attachmentFileName = null;
            _cloudAttachmentProviderMock.Setup(x => x.SaveAttachmentAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .Callback<string, Stream>((name, stream) => attachmentFileName = name)
                .ReturnsAsync("path_in_cloud")
                .Verifiable();

            Attachment attachmentToMap = null;
            _domainModelsMapperMock.Setup(x => x.MapToAttachmentResponse(It.IsAny<Attachment>()))
                .Callback<Attachment>(x => attachmentToMap = x)
                .Returns(new AttachmentResponse())
                .Verifiable();

            var utcNow = DateTimeOffset.UtcNow;
            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(utcNow)
                .Verifiable();

            // Act
            await _messageService.AddMessageAttachmentAsync(request);

            // Assert
            VerifyMocks();

            attachmentToMap.ContentType.Should().Be(request.ContentType);
            attachmentToMap.Created.Should().Be(utcNow);
            attachmentToMap.FileName.Should().Be(attachmentFileName);
            attachmentToMap.MessageId.Should().Be(request.MessageId);
            attachmentToMap.Size.Should().Be(request.Size);
            attachmentFileName.Should().EndWith($".{request.Extension}");
        }
    }
}