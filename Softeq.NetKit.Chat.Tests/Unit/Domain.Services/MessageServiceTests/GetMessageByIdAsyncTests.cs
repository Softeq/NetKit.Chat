// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MessageServiceTests
{
    public class GetMessageByIdAsyncTests : MessageServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfMessageDoesNotExist()
        {
            // Arrange
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Message)null)
                .Verifiable();

            var messageId = new Guid("D7CECB2A-076C-41E3-AB4B-9C0D49292CBA");

            // Act
            Func<Task> act = async () => { await _messageService.GetMessageByIdAsync(messageId); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get message by {nameof(messageId)}. Message {nameof(messageId)}:{messageId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldReturnMessage()
        {
            // Arrange
            var member = new Member
            {
                Id = new Guid("0B1837ED-8E84-450E-9BF1-DFC4AB766DFF"),
                PhotoName = "photo name"
            };
            var message = new Message
            {
                Id = new Guid("7C1F8365-715D-4135-ADD7-B033760947BF"),
                OwnerId = member.Id,
                Owner = member
            };

            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.Is<Guid>(messageId => messageId.Equals(message.Id))))
                .ReturnsAsync(message)
                .Verifiable();

            var messageResponse = new MessageResponse();
            _domainModelsMapperMock.Setup(x => x.MapToMessageResponse(It.Is<Message>(m => m.Equals(message)), It.IsAny<DateTimeOffset?>()))
                .Returns(messageResponse)
                .Verifiable();

            // Act
            await _messageService.GetMessageByIdAsync(message.Id);

            // Assert
            VerifyMocks();
        }
    }
}