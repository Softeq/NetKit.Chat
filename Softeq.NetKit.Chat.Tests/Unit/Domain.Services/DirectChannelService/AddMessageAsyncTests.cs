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
    public class AddMessageAsyncTests : DirectChannelTestBase
    {
        [Fact]
        public void ShouldThrowIfChannelDoesNotExist()
        {
            // Arrange
            var saasUserId = "4761D81E-F700-4BDF-B986-C09FA22D8CF8";
            var directChannelId = new Guid("4359E70A-E6B5-4D18-8010-759678A945EF");

            _directChannelRepositoryMock.Setup(x => x.GetDirectChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync((DirectChannel)null)
                .Verifiable();

            var directMessageRequest = new CreateDirectMessageRequest(saasUserId, directChannelId, MessageType.Default, "TestBody");

            // Act
            Func<Task> act = async () => { await DirectChannelService.AddMessageAsync(directMessageRequest); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should()
                .Be($"Unable to add direct message. Channel {nameof(directMessageRequest.DirectChannelId)}" +
                    $":{directMessageRequest.DirectChannelId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var saasUserId = "4761D81E-F700-4BDF-B986-C09FA22D8CF8";
            var directChannelId = new Guid("4359E70A-E6B5-4D18-8010-759678A945EF");

            _directChannelRepositoryMock.Setup(x => x.GetDirectChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new DirectChannel())
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var directMessageRequest = new CreateDirectMessageRequest(saasUserId, directChannelId, MessageType.Default, "TestBody");

            // Act
            Func<Task> act = async () => { await DirectChannelService.AddMessageAsync(directMessageRequest); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get member. Member SaasUserId:{saasUserId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldReturnDirectMessageResponse()
        {
            // Arrange
            var saasUserId = "4761D81E-F700-4BDF-B986-C09FA22D8CF8";

            var directChannel = new DirectChannel();
            _directChannelRepositoryMock.Setup(x => x.GetDirectChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync(directChannel)
                .Verifiable();

            var member = new Member();
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(m => m.Equals(saasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            var utcNow = DateTimeOffset.UtcNow;
            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(utcNow)
                .Verifiable();
            
            var directMessageRequest = new CreateDirectMessageRequest(saasUserId, new Guid("4359E70A-E6B5-4D18-8010-759678A945EF"), MessageType.Default, "TestBody");

            Message messageToAdd = null;
            _messageRepositoryMock.Setup(x => x.AddMessageAsync(It.IsAny<Message>()))
                .Callback<Message>(m => messageToAdd = m)
                .Returns(Task.CompletedTask)
                .Verifiable();

            var directMessageResponse = new DirectMessageResponse();
            _domainModelsMapperMock
                .Setup(x => x.MapToDirectMessageResponse(It.Is<Message>(dm => dm.Equals(messageToAdd))))
                .Returns(directMessageResponse)
                .Verifiable();

            // Act
            var act = await DirectChannelService.AddMessageAsync(directMessageRequest);

            // Assert
            act.Should().BeEquivalentTo(directMessageResponse);

            VerifyMocks();
        }
    }
}
