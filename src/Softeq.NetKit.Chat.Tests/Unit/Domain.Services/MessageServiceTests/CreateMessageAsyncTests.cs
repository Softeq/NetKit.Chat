// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MessageServiceTests
{
    public class CreateMessageAsyncTests : MessageServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfChannelDoesNotExist()
        {
            // Arrange
            var member = new Member { Id = new Guid("20CAE535-7CE3-4ED4-9D8F-2693953E94D3") };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.IsChannelExistsAndOpenAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false)
                .Verifiable();

            var request = new CreateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), MessageType.Default, "body");

            // Act
            Func<Task> act = async () => { await _messageService.CreateMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to create message. Channel {nameof(request.ChannelId)}:{request.ChannelId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var request = new CreateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), MessageType.Default, "body");

            // Act
            Func<Task> act = async () => { await _messageService.CreateMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to create message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfForwardMessageDoesNotExist()
        {
            // Arrange
            var request = new CreateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), MessageType.Forward, "body")
            {
                ForwardedMessageId = new Guid("1325E3C3-7D3E-4502-9CE8-7D7A902C90EE")
            };

            _channelRepositoryMock.Setup(x => x.IsChannelExistsAndOpenAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true)
                .Verifiable();

            var member = new Member { Id = new Guid("20CAE535-7CE3-4ED4-9D8F-2693953E94D3") };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(member)
                .Verifiable();

            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.Is<Guid>(messageId => messageId.Equals(request.ForwardedMessageId))))
                .ReturnsAsync((Message)null)
                .Verifiable();

            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(DateTimeOffset.UtcNow)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _messageService.CreateMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to create message. Forward message {nameof(request.ForwardedMessageId)}:{request.ForwardedMessageId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldCreateDefaultMessage()
        {
            // Arrange
            var request = new CreateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), MessageType.Default, "body")
            {
                ImageUrl = "ImageUrl"
            };

            _channelRepositoryMock.Setup(x => x.IsChannelExistsAndOpenAsync(It.Is<Guid>(channelId => channelId.Equals(request.ChannelId))))
                .ReturnsAsync(true)
                .Verifiable();

            var member = new Member { Id = new Guid("20CAE535-7CE3-4ED4-9D8F-2693953E94D3") };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId => saasUserId.Equals(request.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _cloudImageProviderMock.Setup(x => x.CopyImageToDestinationContainerAsync(It.IsAny<string>())).ReturnsAsync(string.Empty).Verifiable();

            Message message = null;
            _messageRepositoryMock.Setup(x => x.AddMessageAsync(It.IsAny<Message>()))
                .Callback<Message>(x => message = x)
                .Returns(Task.CompletedTask)
                .Verifiable();

            _channelMemberRepositoryMock.Setup(x => x.SetLastReadMessageAsync(
                    It.Is<Guid>(memberId => memberId.Equals(member.Id)),
                    It.Is<Guid>(channelId => channelId.Equals(request.ChannelId)),
                    It.IsAny<Guid>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var messageId = Guid.Empty;
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .Callback<Guid>(x => messageId = x)
                .ReturnsAsync((Message)null)
                .Verifiable();

            var utcNow = DateTimeOffset.UtcNow;
            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(utcNow)
                .Verifiable();

            var messageResponse = new MessageResponse();
            _domainModelsMapperMock.Setup(x => x.MapToMessageResponse(It.IsAny<Message>(), It.IsAny<DateTimeOffset?>()))
                .Returns(messageResponse)
                .Verifiable();

            // Act
            await _messageService.CreateMessageAsync(request);

            // Assert
            VerifyMocks();

            message.Id.Should().Be(messageId);
            message.ChannelId.Should().Be(request.ChannelId);
            message.OwnerId.Should().Be(member.Id);
            message.Body.Should().Be(request.Body);
            message.Type.Should().Be(request.Type);
            message.ImageUrl.Should().Be(request.ImageUrl);
            message.Created.Should().Be(utcNow);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldCreateForwardMessage()
        {
            // Arrange
            var request = new CreateMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), MessageType.Forward, "body")
            {
                ForwardedMessageId = new Guid("D51A3CC1-80F8-41F4-9291-54372B8A7DAE"),
                ImageUrl = "ImageUrl"
            };

            _channelRepositoryMock.Setup(x => x.IsChannelExistsAndOpenAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true)
                .Verifiable();

            var member = new Member { Id = new Guid("20CAE535-7CE3-4ED4-9D8F-2693953E94D3") };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(member)
                .Verifiable();

            _cloudImageProviderMock.Setup(x => x.CopyImageToDestinationContainerAsync(It.IsAny<string>())).ReturnsAsync(string.Empty).Verifiable();

            Message message = null;
            _messageRepositoryMock.Setup(x => x.AddMessageAsync(It.IsAny<Message>()))
                .Callback<Message>(x => message = x)
                .Returns(Task.CompletedTask)
                .Verifiable();

            _channelMemberRepositoryMock.Setup(x => x.SetLastReadMessageAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => id == request.ForwardedMessageId ? new Message { Id = id } : null)
                .Verifiable();

            var forwardMessageId = Guid.Empty;
            _forwardMessageRepositoryMock.Setup(x => x.AddForwardMessageAsync(It.IsAny<ForwardMessage>()))
                .Callback<ForwardMessage>(x => forwardMessageId = x.Id)
                .Returns(Task.CompletedTask)
                .Verifiable();

            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(DateTimeOffset.UtcNow)
                .Verifiable();

            _domainModelsMapperMock.Setup(x => x.MapToMessageResponse(It.IsAny<Message>(), It.IsAny<DateTimeOffset?>()))
                .Returns(new MessageResponse())
                .Verifiable();

            _domainModelsMapperMock.Setup(x => x.MapToForwardMessage(It.IsAny<Message>()))
                .Returns(new ForwardMessage())
                .Verifiable();

            // Act
            await _messageService.CreateMessageAsync(request);

            // Assert
            VerifyMocks();
            _messageRepositoryMock.Verify(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()), Times.Exactly(2));

            forwardMessageId.Should().NotBeEmpty();
            forwardMessageId.Should().Be(message.ForwardMessageId.Value);
        }
    }
}
