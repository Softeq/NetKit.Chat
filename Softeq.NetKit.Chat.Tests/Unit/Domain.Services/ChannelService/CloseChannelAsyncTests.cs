// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelService
{
    public class CloseChannelAsyncTests : ChannelServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfChannelDoesNotExist()
        {
            // Arrange
            var saasUserId = "A0045650-27E0-4454-8720-A195DE9A96FF";
            var channelId = new Guid("8698C83C-FCDC-4920-8CA7-6D4EDBB14971");

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Channel)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _channelService.CloseChannelAsync(saasUserId, channelId);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to close channel. Channel {nameof(channelId)}:{channelId} not found.");
        }

        [Fact]
        public void ShouldThrowIfChannelAlreadyClosed()
        {
            // Arrange
            var saasUserId = "A0045650-27E0-4454-8720-A195DE9A96FF";
            var channelId = new Guid("8698C83C-FCDC-4920-8CA7-6D4EDBB14971");

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Channel { IsClosed = true })
                .Verifiable();

            // Act
            Func<Task> act = async () => await _channelService.CloseChannelAsync(saasUserId, channelId);

            // Assert
            act.Should().Throw<NetKitChatInvalidOperationException>().And.Message.Should()
                .Be($"Unable to close channel. Channel {nameof(channelId)}:{channelId} already closed.");
        }
    }
}
