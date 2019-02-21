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
    public class LeaveFromChannelAsyncTests : ChannelServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var saasUserId = "A3F16B2A-EEF1-4DAE-8025-D801ED1532A3";
            var channelId = new Guid("8E976A56-C641-40A6-8B19-5AD726FD5BF7");

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelService.LeaveFromChannelAsync(saasUserId, channelId); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to leave from channel. Member {nameof(saasUserId)}:{saasUserId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberDoesNotExistInChannel()
        {
            // Arrange
            var saasUserId = "A3F16B2A-EEF1-4DAE-8025-D801ED1532A3";
            var channelId = new Guid("8E976A56-C641-40A6-8B19-5AD726FD5BF7");

            var member = new Member { Id = new Guid("9BA1C9EB-C40F-4C47-9D00-5D797891D34A") };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasId => saasId.Equals(saasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.IsMemberExistsInChannelAsync(It.Is<Guid>(m => m.Equals(member.Id)), It.Is<Guid>(c => c.Equals(channelId))))
                .ReturnsAsync(false)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelService.LeaveFromChannelAsync(saasUserId, channelId); };

            // Assert
            act.Should().Throw<NetKitChatInvalidOperationException>()
                .And.Message.Should()
                .Be($"Unable to leave from channel. Member {nameof(saasUserId)}:{saasUserId} is not joined to channel {nameof(channelId)}:{channelId}.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldLeaveChannel()
        {
            // Arrange
            var saasUserId = "A3F16B2A-EEF1-4DAE-8025-D801ED1532A3";
            var channelId = new Guid("8E976A56-C641-40A6-8B19-5AD726FD5BF7");

            var member = new Member { Id = new Guid("9BA1C9EB-C40F-4C47-9D00-5D797891D34A") };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasId => saasId.Equals(saasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.IsMemberExistsInChannelAsync(It.Is<Guid>(m => m.Equals(member.Id)), It.Is<Guid>(c => c.Equals(channelId))))
                .ReturnsAsync(true)
                .Verifiable();

            _channelMemberRepositoryMock.Setup(x => x.DeleteChannelMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.DecrementChannelMembersCount(It.Is<Guid>(c => c.Equals(channelId))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _channelService.LeaveFromChannelAsync(saasUserId, channelId);

            // Assert
            VerifyMocks();

            _channelMemberRepositoryMock.Verify(prov => prov.DeleteChannelMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));
        }
    }
}
