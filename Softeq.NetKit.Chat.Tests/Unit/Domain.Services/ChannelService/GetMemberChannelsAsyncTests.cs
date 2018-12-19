// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelService
{
    public class GetMemberChannelsAsyncTests : ChannelServiceTestBase
    {
        [Fact]
        public void InvalidSaasUserId_ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange

            var saasUserId = "33C51F54-8666-417B-AF29-C22ED3E6896E";

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _channelService.GetMemberChannelsAsync(saasUserId);

            //Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                            .Be($"Unable to get member channels. Member {nameof(saasUserId)}:{saasUserId} not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task ValidSaasUserId_GetChannels()
        {
            // Arrange
            var allowedMember = new Member
            {
                SaasUserId = "F2443301-985E-4F91-8DA2-48C27EF5F10E"
            };

            var member = new Member
            {
                Id = new Guid("A1538EB3-4E4C-4E39-BDCB-F617003E4BBF")
            };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId =>saasUserId.Equals(allowedMember.SaasUserId))))
                .ReturnsAsync(member).Verifiable(); ;

          var channels = new List<Channel>();

            _channelRepositoryMock.Setup(x => x.GetAllowedChannelsAsync(It.Is<Guid>(memberId => memberId.Equals(member.Id))))
                .ReturnsAsync(channels).Verifiable();

            // Act
            var result = await _channelService.GetMemberChannelsAsync(allowedMember.SaasUserId);

            //Assert
            result.Should().BeEquivalentTo(channels);
            VerifyMocks();
        }
    }
}
