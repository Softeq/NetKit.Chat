// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.ChannelMember;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelMemberService
{
    public class GetChannelMembersTests : ChannelMemberServiceTestsBase
    {
        [Fact]
        public void ShouldThrowIfChannelIsNotExist()
        {
            // Arrange
            var channelId = new Guid("EFBDF9F0-1E51-4EBC-89F3-D31FFA88F659");

            _channelRepositoryMock.Setup(x => x.IsChannelExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(false)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelMemberService.GetChannelMembersAsync(channelId); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get channel members. Channel {nameof(channelId)}:{channelId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldReturnChannelMemberResponsesCollection()
        {
            // Arrange
            var channelId = new Guid("EFBDF9F0-1E51-4EBC-89F3-D31FFA88F659");

            _channelRepositoryMock.Setup(x => x.IsChannelExistsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(true)
                .Verifiable();

            var channelMembers = new List<ChannelMember>
            {
                new ChannelMember {ChannelId = new Guid("634181BE-924F-4ED3-AF9E-CAA8FEC53A57")}
            };

            _channelMemberRepositoryMock.Setup(x => x.GetChannelMembersAsync(It.Is<Guid>(id => id.Equals(channelId))))
                .ReturnsAsync(channelMembers)
                .Verifiable();

            var channelMemberResponses = new List<ChannelMemberResponse>();

            foreach (var channelMember in channelMembers)
            {
                var channelMemberResponse = new ChannelMemberResponse();
                _domainModelsMapperMock.Setup(x => x.MapToChannelMemberResponse(It.Is<ChannelMember>(c => c.Equals(channelMember))))
                    .Returns(channelMemberResponse)
                    .Verifiable();

                channelMemberResponses.Add(channelMemberResponse);
            }

            // Act
            var act = await _channelMemberService.GetChannelMembersAsync(channelId); ;

            // Assert
            act.Should().BeEquivalentTo(channelMemberResponses);
        }
    }
}
