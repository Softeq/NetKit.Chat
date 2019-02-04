// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MessageServiceTests
{
    public class DeleteMessageAsyncTests : MessageServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfMessageDoesNotExist()
        {
            // Arrange
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Message)null)
                .Verifiable();

            var request = new ArchiveMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            // Act
            Func<Task> act = async () => { await _messageService.ArchiveMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to delete message. Message {nameof(request.MessageId)}:{request.MessageId} is not found.");

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

            var request = new ArchiveMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            // Act
            Func<Task> act = async () => { await _messageService.ArchiveMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to delete message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");

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

            var request = new ArchiveMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            // Act
            Func<Task> act = async () => { await _messageService.ArchiveMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatAccessForbiddenException>()
                .And.Message.Should().Be($"Unable to delete message. Message {nameof(request.MessageId)}:{request.MessageId} owner required.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldDeleteForwardMessageWithoutAttachmentsAndWithoutPreviousMessage()
        {
            // Arrange
            var request = new ArchiveMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            var messageOwnerId = new Guid("F19AD922-B0DB-4686-8CB4-F51902800CAE");
            var message = new Message
            {
                Id = request.MessageId,
                OwnerId = messageOwnerId,
                Type = MessageType.Forward,
                ForwardMessageId = new Guid("C7EFF82F-56FF-4966-9040-E79C90F118A7")
            };

            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .ReturnsAsync(message)
                .Verifiable();
            _messageRepositoryMock.Setup(x => x.GetPreviousMessageAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<DateTimeOffset>()))
                .ReturnsAsync((Message)null)
                .Verifiable();
            _messageRepositoryMock.Setup(x => x.ArchiveMessageAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId => saasUserId.Equals(request.SaasUserId))))
                .ReturnsAsync(new Member { Id = messageOwnerId })
                .Verifiable();

            _channelMemberRepositoryMock.Setup(x => x.UpdateLastReadMessageAsync(
                    It.Is<Guid>(old => old.Equals(message.Id)),
                    It.Is<Guid?>(current => current.Equals(null))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _messageService.ArchiveMessageAsync(request);

            // Assert
            VerifyMocks();
        }
    }
}