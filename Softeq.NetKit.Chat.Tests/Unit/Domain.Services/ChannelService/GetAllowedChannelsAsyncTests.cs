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
    public class GetAllowedChannelsAsyncTests : ChannelServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfNotAllowedChannel()
        {
            // Arrange
            var saasUserId = "5BBD389A-4C26-49D7-94C8-1D739D8202E3";
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _channelService.GetAllowedChannelsAsync(saasUserId);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to get allowed channels. Member {nameof(saasUserId)}:{saasUserId} not found.");

            VerifyMocks();
        }
    }
}
