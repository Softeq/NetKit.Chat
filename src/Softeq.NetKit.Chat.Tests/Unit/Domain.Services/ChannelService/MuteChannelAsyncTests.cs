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
    public class MuteChannelAsyncTests : ChannelServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberIsNotExist()
        {
            // Arrange
            var saasUserId = "7E1FC899-630D-4A00-B0A3-388098AB6CFC";
            var channelId = new Guid("EAD59DEC-5ED7-460A-805C-71AAF83AE3B3");
            var isMuted = false;

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelService.MuteChannelAsync(saasUserId, channelId, isMuted); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to mute channel. Member {nameof(saasUserId)}:{saasUserId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberIsNotInChannel()
        {
            // Arrange
            var channelId = new Guid("EAD59DEC-5ED7-460A-805C-71AAF83AE3B3");
            var isMuted = false;

            var member = new Member()
            {
                Id = new Guid("5E29B496-0396-4C53-B101-5D19EB7082EA"),
                SaasUserId = "7E1FC899-630D-4A00-B0A3-388098AB6CFC"
            };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saas => saas.Equals(member.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.IsMemberExistsInChannelAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelService.MuteChannelAsync(member.SaasUserId, channelId, isMuted); };

            // Assert
            act.Should().Throw<NetKitChatInvalidOperationException>()
                .And.Message.Should().Be($"Unable to mute channel. Member saasUserId:{member.SaasUserId} is not joined channel {nameof(channelId)}:{channelId}.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldChannelIsMuted()
        {
            // Arrange
            var channelId = new Guid("EAD59DEC-5ED7-460A-805C-71AAF83AE3B3");
            var isMuted = false;

            var member = new Member
            {
                Id = new Guid("5E29B496-0396-4C53-B101-5D19EB7082EA"),
                SaasUserId = "7E1FC899-630D-4A00-B0A3-388098AB6CFC"
            };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saas => saas.Equals(member.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.IsMemberExistsInChannelAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true)
                .Verifiable();

            _channelMemberRepositoryMock.Setup(x => x.MuteChannelAsync(
                    It.Is<Guid>(m => m.Equals(member.Id)),
                    It.Is<Guid>(c => c.Equals(channelId)),
                    It.Is<bool>(m => m.Equals(isMuted))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _channelService.MuteChannelAsync(member.SaasUserId, channelId, isMuted);

            // Assert
            VerifyMocks();

            _channelMemberRepositoryMock.Verify(prov => prov.MuteChannelAsync(
                It.Is<Guid>(m => m.Equals(member.Id)),
                It.Is<Guid>(c => c.Equals(channelId)),
                It.Is<bool>(m => m.Equals(isMuted))));
        }
    }
}
