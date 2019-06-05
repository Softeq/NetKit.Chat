// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Channel;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelService
{
    public class GetChannelByIdAsyncTests : ChannelServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfChannelDoesNotExist()
        {
            // Arrange
            var channelId = new Guid("CA3747EC-7410-43C4-B4C8-77D9900451E7");

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Channel)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _channelService.GetChannelByIdAsync(channelId);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to get channel by {nameof(channelId)}. Channel {nameof(channelId)}:{channelId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldReturnChannel()
        {
            // Arrange
            var channel = new Channel { Id = new Guid("3B16890F-4D6E-445D-BF6C-C293F3AF68FE") };

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.Is<Guid>(id => id.Equals(channel.Id))))
                .ReturnsAsync(channel)
                .Verifiable();

            var channelResponse = new ChannelResponse();

            _domainModelsMapperMock.Setup(x => x.MapToChannelResponse(It.Is<Channel>(ch => ch.Equals(channel))))
                .Returns(channelResponse);

            // Act
            var result = await _channelService.GetChannelByIdAsync(channel.Id);

            // Assert
            result.Should().BeEquivalentTo(channelResponse);
        }
    }
}
