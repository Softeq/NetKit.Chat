// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberService;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberServiceTests
{
   public class GetChannelMembersAsyncTests : MemberServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfChannelDoesNotExist()
        {
            // Arrange
            var channelId = new Guid("BE5C68F1-5983-4C08-B57B-FD4EFD7295B8");

            _channelRepositoryMock.Setup(x => x.IsChannelExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _memberService.GetChannelMembersAsync(channelId);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to get channel members. Channel {nameof(channelId)}:{channelId} not found.");

            VerifyMocks();
        }
    }
}
