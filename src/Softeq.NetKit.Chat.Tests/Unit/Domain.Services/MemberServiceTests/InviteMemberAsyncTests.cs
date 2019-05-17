// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberService;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberServiceTests
{
    public class InviteMemberAsyncTests : MemberServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfChannelDoesNotExist()
        {
            // Arrange
            var channelId = new Guid("A83E5C8B-8B3F-43DB-96B6-A86A5941933F");
            var memberId = new Guid("90E9482F-42B3-4F14-8EC6-30DD3B0F8E91");

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Channel)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _memberService.InviteMemberAsync(memberId, channelId);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to invite member. Channel {nameof(channelId)}:{channelId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfChannelIsClosed()
        {
            // Arrange
            var channelId = new Guid("A83E5C8B-8B3F-43DB-96B6-A86A5941933F");
            var memberId = new Guid("90E9482F-42B3-4F14-8EC6-30DD3B0F8E91");

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Channel { IsClosed = true })
                .Verifiable();

            // Act
            Func<Task> act = async () => await _memberService.InviteMemberAsync(memberId, channelId);

            // Assert
            act.Should().Throw<NetKitChatInvalidOperationException>().And.Message.Should()
                .Be($"Unable to invite member. Channel {nameof(channelId)}:{channelId} is closed.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var channelId = new Guid("A83E5C8B-8B3F-43DB-96B6-A86A5941933F");
            var memberId = new Guid("90E9482F-42B3-4F14-8EC6-30DD3B0F8E91");

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Channel { IsClosed = false })
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _memberService.InviteMemberAsync(memberId, channelId);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to invite member. Member {nameof(memberId)}:{memberId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberExistsInChannel()
        {
            // Arrange
            var channelId = new Guid("A83E5C8B-8B3F-43DB-96B6-A86A5941933F");
            var memberId = new Guid("90E9482F-42B3-4F14-8EC6-30DD3B0F8E91");

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Channel { IsClosed = false })
                .Verifiable();

            var member = new Member();
            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.Is<Guid>(m => m.Equals(memberId))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.IsMemberExistsInChannelAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _memberService.InviteMemberAsync(memberId, channelId);

            // Assert
            act.Should().Throw<NetKitChatInvalidOperationException>().And.Message.Should()
                .Be($"Unable to invite member. Member {nameof(memberId)}:{memberId} already joined channel {nameof(channelId)}:{channelId}.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldReturnChannelResponse()
        {
            // Arrange
            var channel = new Channel
            {
                Id = new Guid("02A86CF8-2A22-4381-97D3-5EB0C0F387F0"),
                IsClosed = false
            };
            var member = new Member { Id = new Guid("0AAB1E31-F907-4EEB-808D-77B7E2BFA66B") };

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync(channel)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.Is<Guid>(m => m.Equals(member.Id))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.IsMemberExistsInChannelAsync(It.Is<Guid>(m => m.Equals(member.Id)), It.Is<Guid>(c => c.Equals(channel.Id))))
                .ReturnsAsync(false)
                .Verifiable();

            _channelMemberRepositoryMock.Setup(x => x.AddChannelMemberAsync(It.IsAny<ChannelMember>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.IncrementChannelMembersCountAsync(It.Is<Guid>(c => c.Equals(channel.Id))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.Is<Guid>(c => c.Equals(channel.Id))))
                .ReturnsAsync(channel)
                .Verifiable();

            var channelResponse = new ChannelResponse();
            _domainModelsMapperMock.Setup(x => x.MapToChannelResponse(It.Is<Channel>(c => c.Equals(channel))))
                .Returns(channelResponse)
                .Verifiable();

            // Act
            var act = await _memberService.InviteMemberAsync(member.Id, channel.Id);

            // Assert
            act.Should().BeEquivalentTo(channelResponse);
        }
    }
}
