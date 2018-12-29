// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using ServiceStack;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelService
{
    public class PinChannelAsyncTests : ChannelServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var saasUserId = "33C51F54-8666-417B-AF29-C22ED3E6896E";
            var channelId = new Guid("D006767E-C089-4441-B674-F55F6D301ED9");

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _channelService.PinChannelAsync(saasUserId, channelId, true);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to pin channel. Member {nameof(saasUserId)}:{saasUserId} not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfMemberDoesNotInChannel()
        {
            // Arrange
            var channelId = new Guid("D006767E-C089-4441-B674-F55F6D301ED9");

            var member = new Member
            {
                Id = new Guid("7657C4D0-D5D3-4F8A-97E0-70AC1CEAF895"),
                SaasUserId = "33C51F54-8666-417B-AF29-C22ED3E6896E"
            };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saas => saas.Equals(member.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x =>
                x.IsMemberExistsInChannelAsync(It.Is<Guid>(memberId => memberId.Equals(member.Id)),
                    It.Is<Guid>(c => c.Equals(channelId))))
                .ReturnsAsync(false)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _channelService.PinChannelAsync(member.SaasUserId, channelId, true);

            // Assert
            act.Should().Throw<NetKitChatInvalidOperationException>().And.Message.Should()
                .Be($"Unable to pin channel. Member saasUserId:{member.SaasUserId} is not joined channel channelId:{channelId}.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldReturnTask()
        {
            // Arrange
            var channelId = new Guid("D006767E-C089-4441-B674-F55F6D301ED9");

            var member = new Member
            {
                Id = new Guid("7657C4D0-D5D3-4F8A-97E0-70AC1CEAF895"),
                SaasUserId = "33C51F54-8666-417B-AF29-C22ED3E6896E"
            };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saas => saas.Equals(member.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x =>
                    x.IsMemberExistsInChannelAsync(It.Is<Guid>(memberId => memberId.Equals(member.Id)),
                        It.Is<Guid>(c => c.Equals(channelId))))
                .ReturnsAsync(true)
                .Verifiable();

            _channelMemberRepositoryMock.Setup(x => x.PinChannelAsync(It.Is<Guid>(memberId => memberId.Equals(member.Id)), It.Is<Guid>(c => c.Equals(channelId)), true))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result =  _channelService.PinChannelAsync(member.SaasUserId, channelId, true);

            // Assert
            VerifyMocks();

            await result.Should().AsTaskResult();
        }
    }
}
