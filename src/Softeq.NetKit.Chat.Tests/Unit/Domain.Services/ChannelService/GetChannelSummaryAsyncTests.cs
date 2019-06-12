// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Member;
using Xunit;
using Channel = Softeq.NetKit.Chat.Domain.DomainModels.Channel;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelService
{
    public class GetChannelSummaryAsyncTests : ChannelServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldThrowIfChannelDoesNotExist()
        {
            // Arrange
            var saasUserId = "2A70F115-9F55-4024-829B-6521FE18680C";
            var channelId = new Guid("9645F17A-1050-48AB-B3D9-129B369D22F3");

            _channelRepositoryMock.Setup(x => x.GetChannelWithCreatorAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Channel)null)
                .Verifiable();

            // Act
            Func<Task> result = async () => { await _channelService.GetChannelSummaryAsync(saasUserId, channelId); };

            // Assert
            result.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get channel summary. Channel {nameof(channelId)}:{channelId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldThrowIfMemberSummaryResponseDoesNotExist()
        {
            // Arrange
            var allowedChannel = new Channel { Id = new Guid("4C13BEC1-2979-4822-9AAC-520B474214FD") };
            var saasUserId = "2A70F115-9F55-4024-829B-6521FE18680C";

            _channelRepositoryMock.Setup(x => x.GetChannelWithCreatorAsync(It.Is<Guid>(channel => channel.Equals(allowedChannel.Id))))
                .ReturnsAsync(allowedChannel)
                .Verifiable();

            _memberServiceMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((MemberSummaryResponse)null)
                .Verifiable();

            var messages = new List<Message>();
            _messageRepositoryMock.Setup(x => x.GetAllChannelMessagesWithOwnersAsync(It.IsAny<Guid>()))
                .ReturnsAsync(messages)
                .Verifiable();

            // Act
            Func<Task> result = async () => { await _channelService.GetChannelSummaryAsync(saasUserId, allowedChannel.Id); };

            // Assert
            result.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get channel summary. Member { nameof(saasUserId)}:{ saasUserId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldReturnChannelSummaryResponse()
        {
            // Arrange
            var allowedChannel = new Channel { Id = new Guid("4C13BEC1-2979-4822-9AAC-520B474214FD") };
            var saasUserId = "2A70F115-9F55-4024-829B-6521FE18680C";

            _channelRepositoryMock.Setup(x => x.GetChannelWithCreatorAsync(It.Is<Guid>(channel => channel.Equals(allowedChannel.Id))))
                .ReturnsAsync(allowedChannel)
                .Verifiable();

            var member = new MemberSummaryResponse();
            _memberServiceMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(member)
                .Verifiable();

            var allowedChannelMember = new ChannelMember();
            _channelMemberRepositoryMock.Setup(x => x.GetChannelMemberAsync(
                    It.Is<Guid>(memberId => memberId.Equals(member.Id)),
                    It.Is<Guid>(channelId => channelId.Equals(allowedChannel.Id))))
                .ReturnsAsync(allowedChannelMember)
                .Verifiable();

            var lastReadMessage = new Message();
            _messageRepositoryMock.Setup(x => x.GetLastReadMessageAsync(
                    It.Is<Guid>(memberId => memberId.Equals(member.Id)),
                    It.Is<Guid>(channelId => channelId.Equals(allowedChannel.Id))))
                .ReturnsAsync(lastReadMessage)
                .Verifiable();

            var channelSummaryResponse = new ChannelSummaryResponse();
            _domainModelsMapperMock.Setup(x => x.MapToChannelSummaryResponse(
                    It.Is<Channel>(channel => channel.Equals(allowedChannel)),
                    It.Is<ChannelMember>(channelMember => channelMember.Equals(allowedChannelMember)),
                    It.Is<Message>(message => message.Equals(lastReadMessage))))
                .Returns(channelSummaryResponse)
                .Verifiable();

            var messages = new List<Message>();
            _messageRepositoryMock.Setup(x => x.GetAllChannelMessagesWithOwnersAsync(It.IsAny<Guid>()))
                .ReturnsAsync(messages)
                .Verifiable();

            // Act
            var result = await _channelService.GetChannelSummaryAsync(saasUserId, allowedChannel.Id);

            // Assert
            VerifyMocks();
            result.Should().BeEquivalentTo(channelSummaryResponse);
        }
    }
}
