// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Tests.Abstract;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.RepositoryTests
{
    public class ChannelMemberRepositoryTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("549cf269-53b5-46a5-b9f0-4138d45ccec0");
        private readonly Guid _channelId = new Guid("9a5be1d6-4b75-4190-9678-cc59f493252e");

        public ChannelMemberRepositoryTests()
        {
            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Active,
                Role = UserRole.User
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            var channel = new Channel
            {
                Id = _channelId,
                CreatorId = member.Id,
                Name = "testChannelMember",
                MembersCount = 0,
                Type = ChannelType.Public
            };
            UnitOfWork.ChannelRepository.AddChannelAsync(channel).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task AddChannelMemberAsyncTest()
        {
            // Arrange
            var channelMember = new ChannelMembers
            {
                MemberId = _memberId,
                ChannelId = _channelId,
                LastReadMessageId = null
            };

            // Act
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
            var newChannelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(channelMember.MemberId, channelMember.ChannelId);

            // Assert
            Assert.NotNull(newChannelMember);

            Assert.Equal(channelMember.MemberId, newChannelMember.MemberId);
            Assert.Equal(channelMember.ChannelId, newChannelMember.ChannelId);
            Assert.Equal(channelMember.IsMuted, newChannelMember.IsMuted);
            Assert.Equal(channelMember.LastReadMessageId, newChannelMember.LastReadMessageId);
        }

        [Fact]
        public async Task DeleteChannelMemberAsyncTest()
        {
            // Arrange
            var channelMember = new ChannelMembers
            {
                MemberId = _memberId,
                ChannelId = _channelId,
                LastReadMessageId = null
            };

            // Act
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
            await UnitOfWork.ChannelMemberRepository.DeleteChannelMemberAsync(channelMember.MemberId, channelMember.ChannelId);
            var newChannelMembers = await UnitOfWork.ChannelMemberRepository.GetChannelMembersAsync(channelMember.ChannelId);

            // Assert
            Assert.Empty(newChannelMembers);
        }

        [Fact]
        public async Task GetChannelMemberByIdAsync()
        {
            // Arrange
            var channelMember = new ChannelMembers
            {
                MemberId = _memberId,
                ChannelId = _channelId,
                LastReadMessageId = null
            };

            // Act
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
            var newChannelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(channelMember.MemberId, channelMember.ChannelId);

            // Assert
            Assert.NotNull(newChannelMember);
            Assert.Equal(channelMember.MemberId, newChannelMember.MemberId);
            Assert.Equal(channelMember.ChannelId, newChannelMember.ChannelId);
            Assert.Equal(channelMember.IsMuted, newChannelMember.IsMuted);
        }

        [Fact]
        public async Task PinChannelAsync_ShouldChangeIsPinnedStatus()
        {
            var channelMember = new ChannelMembers
            {
                MemberId = _memberId,
                ChannelId = _channelId,
                LastReadMessageId = null,
                IsPinned = false
            };

            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
            await UnitOfWork.ChannelMemberRepository.PinChannelAsync(_memberId, _channelId);

            var pinnedChannelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(_memberId, _channelId);
            pinnedChannelMember.IsPinned.Should().BeTrue();

            await UnitOfWork.ChannelMemberRepository.PinChannelAsync(_memberId, _channelId);
            var unPinnedChannelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(_memberId, _channelId);
            unPinnedChannelMember.IsPinned.Should().BeFalse();
        }
    }
}