// Developed by Softeq Development Corporation
// http://www.softeq.com

using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Channel;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelService
{
    public class GetAllChannelsAsyncTests : ChannelServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldReturnChannelResponses()
        {
            // Arrange
            var channels = new List<Channel>();
            _channelRepositoryMock.Setup(x => x.GetAllChannelsAsync())
                .ReturnsAsync(channels)
                .Verifiable();

            var channelResponses = new List<ChannelResponse>();
            foreach (var channel in channels)
            {
                var channelResponse = new ChannelResponse();
                _domainModelsMapperMock.Setup(x => x.MapToChannelResponse(It.Is<Channel>(c => c.Equals(channel))))
                    .Returns(channelResponse)
                    .Verifiable();

                channelResponses.Add(channelResponse);
            }

            // Act
           var act = await _channelService.GetAllChannelsAsync();

            // Assert
            VerifyMocks();

            act.Should().AllBeEquivalentTo(channelResponses);
        }
    }
}
