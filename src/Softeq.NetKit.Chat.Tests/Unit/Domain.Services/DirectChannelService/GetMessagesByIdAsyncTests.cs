// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.DirectChannelService
{
    public class GetMessagesByIdAsyncTests : DirectChannelTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfDirectMessageDoesNotExist()
        {
            // Arrange
            var messageId = new Guid("4C21A8B9-75CD-43BA-9F18-2D86D479E9F0");

            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Message) null)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await DirectChannelService.GetMessageAsync(messageId); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get direct message. Message with {nameof(messageId)}:{messageId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var messageId = new Guid("4C21A8B9-75CD-43BA-9F18-2D86D479E9F0");
            var directChannelId = new Guid("7626270A-FC98-4C36-A717-FB12128D723D");

            var directMessage = new Message { DirectChannelId = directChannelId };

            _messageRepositoryMock.Setup(x =>x.GetMessageWithOwnerAndForwardMessageAsync(It.Is<Guid>(id =>id.Equals(messageId))))
                .ReturnsAsync(directMessage)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await DirectChannelService.GetMessageAsync(messageId); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get member. Member { nameof(directMessage.OwnerId) }:{ directMessage.OwnerId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldReturnDirectMessage()
        {
            // Arrange
            var messageId = new Guid("4C21A8B9-75CD-43BA-9F18-2D86D479E9F0");
            var directChannelId = new Guid("7626270A-FC98-4C36-A717-FB12128D723D");

            var directMessage = new Message { DirectChannelId = directChannelId };

            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.Is<Guid>(id => id.Equals(messageId))))
                .ReturnsAsync(directMessage)
                .Verifiable();

            var member = new Member();
            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(member)
                .Verifiable();

            var directMessageResponse = new DirectMessageResponse();
            _domainModelsMapperMock.Setup(x =>x.MapToDirectMessageResponse(It.Is<Message>(dm =>dm.Equals(directMessage))))
                .Returns(directMessageResponse)
                .Verifiable();

            // Act
            var act = await DirectChannelService.GetMessageAsync(messageId);

            // Assert
            act.Should().BeEquivalentTo(directMessageResponse);
        }
    }
}
