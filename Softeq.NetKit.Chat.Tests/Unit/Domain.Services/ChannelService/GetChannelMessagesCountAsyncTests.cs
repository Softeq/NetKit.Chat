// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelService
{
    public class GetChannelMessagesCountAsyncTests : ChannelServiceTestBase
    {
        [Fact]

        public async Task ShouldReturnInt()
        {
            //Arrange
            var channelId = new Guid("EAD59DEC-5ED7-460A-805C-71AAF83AE3B3");

            var counter = 0;

            for (int i = 0; i < 100; i++)
            {
                _messageRepositoryMock.Setup(x => x.GetChannelMessagesCountAsync(It.IsAny<Guid>()))
                    .Callback(() => { counter += 1; })
                    .ReturnsAsync(counter);

                // Act
                var result = await _channelService.GetChannelMessagesCountAsync(channelId);

                //Assert
                result.Equals(counter);

            }
        }
    }
}
