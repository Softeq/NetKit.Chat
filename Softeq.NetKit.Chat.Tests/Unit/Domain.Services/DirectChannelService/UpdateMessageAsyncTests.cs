// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.DirectChannelService
{
    public class UpdateMessageAsyncTests : DirectChannelTestBase
    {
        [Fact]
        public void ShouldThrowIfMessageDoesNotExist()
        {
            // Arrange
            var messageId = new Guid("DA1D61EE-420F-4F8E-82AD-F2158358B0E0");
            var saasUserId = "BD0F3206-37AF-43E4-90B8-C302CB74DC02";
            var directChannelId = new Guid("0285A417-E6F4-402D-942E-E808F568EF43");

            _directMessagesRepository.Setup(x => x.GetMessageByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((DirectMessage)null)
                .Verifiable();

            var directMessage = new UpdateDirectMessageRequest(saasUserId, messageId, directChannelId, "NewTestBody");

            // Act
            Func<Task> act = async () => { await DirectChannelService.UpdateMessageAsync(directMessage); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get direct message. Message with {nameof(directMessage.MessageId)}:{directMessage.MessageId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var messageId = new Guid("DA1D61EE-420F-4F8E-82AD-F2158358B0E0");
            var saasUserId = "BD0F3206-37AF-43E4-90B8-C302CB74DC02";
            var directChannelId = new Guid("0285A417-E6F4-402D-942E-E808F568EF43");

            var updateDirectMessageRequest = new UpdateDirectMessageRequest(saasUserId, messageId, directChannelId, "NewTestBody");

            var directMessage = new DirectMessage
            {
                Id = messageId,
                DirectChannelId = new Guid("0285A417-E6F4-402D-942E-E808F568EF43"),
                OwnerId = new Guid("B7AF30DD-A06C-4621-A98F-2F4E9FB8076A")
            };

            _directMessagesRepository.Setup(x => x.GetMessageByIdAsync(It.Is<Guid>(m => m.Equals(messageId))))
                .ReturnsAsync(directMessage)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await DirectChannelService.UpdateMessageAsync(updateDirectMessageRequest); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get member. Member { nameof(directMessage.OwnerId) }:{ directMessage.OwnerId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldReturnDirectMessageResponse()
        {
            // Arrange
            var messageId = new Guid("DA1D61EE-420F-4F8E-82AD-F2158358B0E0");
            var saasUserId = "BD0F3206-37AF-43E4-90B8-C302CB74DC02";
            var directChannelId = new Guid("0285A417-E6F4-402D-942E-E808F568EF43");
            var body = "TestBody";

            var updateDirectMessageRequest = new UpdateDirectMessageRequest(saasUserId, messageId, directChannelId, "NewTestBody");

            var directMessage = new DirectMessage();
            _directMessagesRepository.Setup(x => x.GetMessageByIdAsync(It.Is<Guid>(m => m.Equals(messageId))))
                .ReturnsAsync(directMessage)
                .Verifiable();

            var member = new Member();
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(id => id.Equals(saasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            var utcNow = DateTimeOffset.UtcNow;
            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(utcNow)
                .Verifiable();

            DirectMessage messageToAdd = null;
            _directMessagesRepository.Setup(x => x.UpdateMessageAsync(It.IsAny<DirectMessage>()))
                .Callback<DirectMessage>(x => messageToAdd = x)
                .Returns(Task.CompletedTask)
                .Verifiable();

            var directMessageResponse = new DirectMessageResponse();
            _domainModelsMapperMock
                .Setup(x => x.MapToDirectMessageResponse(It.Is<DirectMessage>(dm => dm.Equals(messageToAdd)), It.Is<Member>(m => m.Equals(member))))
                .Returns(directMessageResponse)
                .Verifiable();

            // Act
            var act = await DirectChannelService.UpdateMessageAsync(updateDirectMessageRequest);

            // Assert
            act.Should().BeEquivalentTo(directMessageResponse);

            VerifyMocks();
        }
    }
}
