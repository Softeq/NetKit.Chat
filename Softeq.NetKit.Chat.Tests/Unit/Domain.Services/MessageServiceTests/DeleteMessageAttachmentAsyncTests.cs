// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MessageServiceTests
{
    public class DeleteMessageAttachmentAsyncTests : MessageServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfMessageDoesNotExist()
        {
            // Arrange
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Message)null)
                .Verifiable();

            var request = new DeleteMessageAttachmentRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), new Guid("609B190D-063C-4C2E-A7BD-5E8F98E8D4D9"));

            // Act
            Func<Task> act = async () => { await _messageService.DeleteMessageAttachmentAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to delete message attachment. Message {nameof(request.MessageId)}:{request.MessageId} is not found.");

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

            var request = new DeleteMessageAttachmentRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), new Guid("609B190D-063C-4C2E-A7BD-5E8F98E8D4D9"));

            // Act
            Func<Task> act = async () => { await _messageService.DeleteMessageAttachmentAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to delete message attachment. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");

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

            var request = new DeleteMessageAttachmentRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), new Guid("609B190D-063C-4C2E-A7BD-5E8F98E8D4D9"));

            // Act
            Func<Task> act = async () => { await _messageService.DeleteMessageAttachmentAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatAccessForbiddenException>()
                .And.Message.Should().Be($"Unable to delete message attachment. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfAttachmentNotFound()
        {
            // Arrange
            var request = new DeleteMessageAttachmentRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), new Guid("609B190D-063C-4C2E-A7BD-5E8F98E8D4D9"));

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

            _attachmentRepositoryMock.Setup(x => x.GetAttachmentAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Attachment)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _messageService.DeleteMessageAttachmentAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to delete message attachment. Attachment {nameof(request.AttachmentId)}:{request.AttachmentId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfMessageDoesNotContainAttachment()
        {
            // Arrange
            var request = new DeleteMessageAttachmentRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), new Guid("609B190D-063C-4C2E-A7BD-5E8F98E8D4D9"));

            var member = new Member
            {
                Id = new Guid("ABF2CA08-5374-4CED-BE87-6EA93A8B90DA")
            };

            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Message { Id = new Guid("5684E61A-05B2-46C6-97DC-5136AC6D68CE"), OwnerId = member.Id })
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(member)
                .Verifiable();

            _attachmentRepositoryMock.Setup(x => x.GetAttachmentAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Attachment { MessageId = new Guid("6AB05E21-5EEE-4407-B798-886D2ED04A4C") })
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _messageService.DeleteMessageAttachmentAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatInvalidOperationException>()
                .And.Message.Should().Be($"Unable to delete message attachment. Message {nameof(request.MessageId)}:{request.MessageId} does not contain attachment {nameof(request.AttachmentId)}:{request.AttachmentId}.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldDeleteAttachment()
        {
            // Arrange
            var request = new DeleteMessageAttachmentRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), new Guid("609B190D-063C-4C2E-A7BD-5E8F98E8D4D9"));

            var member = new Member
            {
                Id = new Guid("ABF2CA08-5374-4CED-BE87-6EA93A8B90DA")
            };

            var message = new Message
            {
                Id = new Guid("5684E61A-05B2-46C6-97DC-5136AC6D68CE"),
                OwnerId = member.Id
            };

            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .ReturnsAsync(message)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId => saasUserId.Equals(request.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            var attachment = new Attachment
            {
                Id = request.AttachmentId,
                MessageId = message.Id,
                FileName = "attachment file name"
            };

            _attachmentRepositoryMock.Setup(x => x.GetAttachmentAsync(It.Is<Guid>(attachmentId => attachmentId.Equals(request.AttachmentId))))
                .ReturnsAsync(attachment)
                .Verifiable();
            
            _attachmentRepositoryMock.Setup(x => x.DeleteAttachmentAsync(It.Is<Guid>(attachmentId => attachmentId.Equals(request.AttachmentId))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _cloudAttachmentProviderMock.Setup(x => x.DeleteMessageAttachmentAsync(It.Is<string>(attachmentFileName => attachmentFileName.Equals(attachment.FileName))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _messageService.DeleteMessageAttachmentAsync(request);

            // Assert
            VerifyMocks();
        }
    }
}