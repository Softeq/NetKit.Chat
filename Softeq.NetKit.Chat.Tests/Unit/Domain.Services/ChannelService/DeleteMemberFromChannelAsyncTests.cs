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
    public class DeleteMemberFromChannelAsyncTests : ChannelServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfMemberIsNotExist()
        {
            // Arrange
            var saasUserId = "7E1FC899-630D-4A00-B0A3-388098AB6CFC";
            var channelId = new Guid("EAD59DEC-5ED7-460A-805C-71AAF83AE3B3");
            var memberToDeleteId = new Guid("44D830F2-87A0-4CC3-9757-A87D575E3CD3");

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelService.DeleteMemberFromChannelAsync(saasUserId, channelId, memberToDeleteId); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to delete member from channel. Member {nameof(saasUserId)}:{saasUserId} not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfMemberIdEqualsMemberToDeleteId()
        {
            // Arrange
            var channelId = new Guid("EAD59DEC-5ED7-460A-805C-71AAF83AE3B3");
            var memberToDeleteId = new Guid("44D830F2-87A0-4CC3-9757-A87D575E3CD3");

            var member = new Member
            {
                Id = new Guid("44D830F2-87A0-4CC3-9757-A87D575E3CD3"),
                SaasUserId = "7E1FC899-630D-4A00-B0A3-388098AB6CFC"
            };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(m => m.Equals(member.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelService.DeleteMemberFromChannelAsync(member.SaasUserId, channelId, memberToDeleteId); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to delete member from channel. Unable to delete yourself. Use {nameof(_channelService.LeaveFromChannelAsync)} method instead.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfChannelDoesNotExist()
        {
            // Arrange
            var channelId = new Guid("EAD59DEC-5ED7-460A-805C-71AAF83AE3B3");
            var memberToDeleteId = new Guid("44D830F2-87A0-4CC3-9757-A87D575E3CD3");

            var member = new Member
            {
                Id = new Guid("44E00AA7-E25B-44CF-80DF-A912133E461E"),
                SaasUserId = "7E1FC899-630D-4A00-B0A3-388098AB6CFC"
            };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(m => m.Equals(member.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Channel)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelService.DeleteMemberFromChannelAsync(member.SaasUserId, channelId, memberToDeleteId); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to delete member from channel. Channel {nameof(channelId)}:{channelId} not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfMemberIdIsNotEqualToChannelCreatorId()
        {
            // Arrange
            var member = new Member
            {
                Id = new Guid("DB558B61-CC6B-4DDF-9DBC-6DA95A7D5869"),
                SaasUserId = "3750EC03-B83C-4391-8398-157C678A8D34"
            };

            var memberToDeleteId = new Guid("229E9C8B-707D-48CA-9D57-37F1687B4C93");

            var channel = new Channel
            {
                Id = new Guid("1A35BB6C-D80F-4275-9FA7-DD14C8E75AD5"),
                CreatorId = new Guid("0EFC3AE1-5BB1-4FBE-8A75-ECCA1C024992")
            };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(m => m.Equals(member.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.Is<Guid>(c => c.Equals(channel.Id))))
                .ReturnsAsync(channel)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelService.DeleteMemberFromChannelAsync(member.SaasUserId, channel.Id, memberToDeleteId); };

            // Assert
            act.Should().Throw<NetKitChatAccessForbiddenException>()
                .And.Message.Should().Be($"Unable to delete member from channel. Channel channelId:{channel.Id} owner required.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfMemberNotInChannel()
        {
            // Arrange
            var member = new Member
            {
                Id = new Guid("DB558B61-CC6B-4DDF-9DBC-6DA95A7D5869"),
                SaasUserId = "3750EC03-B83C-4391-8398-157C678A8D34"
            };

            var memberToDeleteId = new Guid("229E9C8B-707D-48CA-9D57-37F1687B4C93");

            var channel = new Channel
            {
                Id = new Guid("1A35BB6C-D80F-4275-9FA7-DD14C8E75AD5"),
                CreatorId = new Guid("DB558B61-CC6B-4DDF-9DBC-6DA95A7D5869")
            };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(m => m.Equals(member.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.Is<Guid>(c => c.Equals(channel.Id))))
                .ReturnsAsync(channel)
                .Verifiable();

            _channelRepositoryMock.Setup(x =>
                x.IsMemberExistsInChannelAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelService.DeleteMemberFromChannelAsync(member.SaasUserId, channel.Id, memberToDeleteId); };

            // Assert
            act.Should().Throw<NetKitChatInvalidOperationException>()
                .And.Message.Should().Be($"Unable to delete member from channel. Member {nameof(memberToDeleteId)}:{memberToDeleteId} is not joined to channel channelId:{channel.Id}.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldDeleteMemberFromChannel()
        {
            // Arrange
            var member = new Member
            {
                Id = new Guid("DB558B61-CC6B-4DDF-9DBC-6DA95A7D5869"),
                SaasUserId = "3750EC03-B83C-4391-8398-157C678A8D34"
            };

            var memberToDeleteId = new Guid("229E9C8B-707D-48CA-9D57-37F1687B4C93");

            var channel = new Channel
            {
                Id = new Guid("1A35BB6C-D80F-4275-9FA7-DD14C8E75AD5"),
                CreatorId = new Guid("DB558B61-CC6B-4DDF-9DBC-6DA95A7D5869")
            };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(m => m.Equals(member.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.Is<Guid>(c => c.Equals(channel.Id))))
                .ReturnsAsync(channel)
                .Verifiable();

            _channelRepositoryMock.Setup(x =>
                    x.IsMemberExistsInChannelAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true)
                .Verifiable();

            _channelMemberRepositoryMock.Setup(x => x.DeleteChannelMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.DecrementChannelMembersCount(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _channelService.DeleteMemberFromChannelAsync(member.SaasUserId, channel.Id, memberToDeleteId);

            // Assert
            VerifyMocks();

            _channelMemberRepositoryMock.Verify(prov => prov.DeleteChannelMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>()));
        }
    }
}
