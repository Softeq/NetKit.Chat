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
    public class JoinToChannelAsyncTests : ChannelServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfChannelDoesNotExist()
        {
            // Arrange
            var saasUserId = "7E1FC899-630D-4A00-B0A3-388098AB6CFC";
            var channelId = new Guid("EAD59DEC-5ED7-460A-805C-71AAF83AE3B3");

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Channel)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelService.JoinToChannelAsync(saasUserId, channelId); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to join channel. Channel {nameof(channelId)}:{channelId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var saasUserId = "83C5161D-29B6-4424-B27C-EA0FB8C577A7";
            var channel = new Channel { Id = new Guid("4983374C-FE15-446B-A5CA-3990E5C878C6") };

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.Is<Guid>(channelId => channelId.Equals(channel.Id))))
                .ReturnsAsync(channel)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelService.JoinToChannelAsync(saasUserId, channel.Id); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to join channel. Member {nameof(saasUserId)}:{saasUserId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberIdDoesNotEqualCreatorId()
        {
            // Arrange
            var saasUserId = "F8C90628-2DC2-4655-94E3-23B70E67A5DA";

            var channel = new Channel
            {
                Id = new Guid("4983374C-FE15-446B-A5CA-3990E5C878C6"),
                CreatorId = new Guid("8519F915-4042-4EE0-9902-9A43CC8E58F8"),
                Type = ChannelType.Private
            };

            var member = new Member { Id = new Guid("34C24130-CD3B-4E96-AEDF-63C440E34CB1") };

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.Is<Guid>(channelId => channelId.Equals(channel.Id))))
                .ReturnsAsync(channel)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasId => saasId.Equals(saasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelService.JoinToChannelAsync(saasUserId, channel.Id); };

            // Assert
            act.Should().Throw<NetKitChatAccessForbiddenException>()
                .And.Message.Should().Be($"Unable to join private channel.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberExistsInChannel()
        {
            // Arrange
            var saasUserId = "F8C90628-2DC2-4655-94E3-23B70E67A5DA";

            var channel = new Channel
            {
                Id = new Guid("4983374C-FE15-446B-A5CA-3990E5C878C6"),
                CreatorId = new Guid("34C24130-CD3B-4E96-AEDF-63C440E34CB1"),
                Type = ChannelType.Public
            };

            var member = new Member { Id = new Guid("34C24130-CD3B-4E96-AEDF-63C440E34CB1") };

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.Is<Guid>(channelId => channelId.Equals(channel.Id))))
                .ReturnsAsync(channel)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasId => saasId.Equals(saasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.IsMemberExistsInChannelAsync(
                    It.Is<Guid>(memberId => member.Id.Equals(member.Id)),
                    It.Is<Guid>(channelId => channelId.Equals(channel.Id))))
                .ReturnsAsync(true)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelService.JoinToChannelAsync(saasUserId, channel.Id); };

            // Assert
            act.Should().Throw<NetKitChatInvalidOperationException>()
                .And.Message.Should().Be($"Unable to join channel. Member {nameof(member.Id)}:{member.Id} already joined channel channelId:{channel.Id}.");
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldAddChannelMember()
        {
            // Arrange
            var saasUserId = "F8C90628-2DC2-4655-94E3-23B70E67A5DA";

            var channel = new Channel
            {
                Id = new Guid("4983374C-FE15-446B-A5CA-3990E5C878C6"),
                CreatorId = new Guid("34C24130-CD3B-4E96-AEDF-63C440E34CB1"),
                Type = ChannelType.Public
            };

            var member = new Member { Id = new Guid("34C24130-CD3B-4E96-AEDF-63C440E34CB1") };

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.Is<Guid>(channelId => channelId.Equals(channel.Id))))
                .ReturnsAsync(channel)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasId => saasId.Equals(saasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.IsMemberExistsInChannelAsync(
                    It.Is<Guid>(memberId => member.Id.Equals(member.Id)),
                    It.Is<Guid>(channelId => channelId.Equals(channel.Id))))
                .ReturnsAsync(false)
                .Verifiable();

            _channelMemberRepositoryMock.Setup(x => x.AddChannelMemberAsync(It.IsAny<ChannelMember>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.IncrementChannelMembersCountAsync(It.Is<Guid>(c => c.Equals(channel.Id))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _channelService.JoinToChannelAsync(saasUserId, channel.Id);

            // Assert
            VerifyMocks();

            _channelMemberRepositoryMock.Verify(prov => prov.AddChannelMemberAsync(It.IsAny<ChannelMember>()));
        }
    }
}
