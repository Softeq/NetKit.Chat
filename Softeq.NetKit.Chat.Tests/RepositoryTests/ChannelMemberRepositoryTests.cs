// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Tests.Abstract;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.RepositoryTests
{
    public class ChannelMemberRepositoryTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("572F5C34-65E7-4139-A16F-2436971031A6");
        private readonly Guid _secondMemberId = new Guid("CCAF9EBE-CDDC-41A6-BD3E-A7EB174135FA");
        private readonly Guid _channelId = new Guid("1F9E6797-DCB7-4477-9BDD-6B7F307DC411");
        private readonly Guid _lastReadMessageId = new Guid("15ECBDAE-FFD3-4D2A-95DC-87831A41FA10");

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

            var secondMember = new Member
            {
                Id = _secondMemberId,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Active,
                Role = UserRole.User
            };
            UnitOfWork.MemberRepository.AddMemberAsync(secondMember).GetAwaiter().GetResult();

            var channel = new Channel
            {
                Id = _channelId,
                CreatorId = member.Id,
                Name = "testChannelMember",
                MembersCount = 0,
                Type = ChannelType.Public
            };
            UnitOfWork.ChannelRepository.AddChannelAsync(channel).GetAwaiter().GetResult();

            var lastReadMessage = new Message
            {
                Id = _lastReadMessageId,
                Body = "message body",
                Created = DateTimeOffset.Now,
                ImageUrl = "test",
                Type = 0,
                ChannelId = _channelId,
                OwnerId = _memberId
            };
            UnitOfWork.MessageRepository.AddMessageAsync(lastReadMessage).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task AddChannelMemberAsync_ShouldAddChannelMember()
        {
            var channelMember = new ChannelMember
            {
                MemberId = _memberId,
                ChannelId = _channelId,
                LastReadMessageId = _lastReadMessageId,
                IsMuted = true,
                IsPinned = true
            };

            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);

            var newChannelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(channelMember.MemberId, channelMember.ChannelId);

            newChannelMember.Should().BeEquivalentTo(channelMember);
        }

        [Fact]
        public async Task GetChannelMemberAsync_ShouldReturnChannelMember()
        {
            var channelMember = new ChannelMember
            {
                MemberId = _memberId,
                ChannelId = _channelId,
                LastReadMessageId = _lastReadMessageId,
                IsMuted = true,
                IsPinned = true
            };

            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);

            var newChannelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(channelMember.MemberId, channelMember.ChannelId);

            newChannelMember.Should().BeEquivalentTo(channelMember);
        }

        [Fact]
        public async Task DeleteChannelMemberAsync_ShouldDeleteChannelMember()
        {
            var channelMember = new ChannelMember
            {
                MemberId = _memberId,
                ChannelId = _channelId
            };

            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);

            await UnitOfWork.ChannelMemberRepository.DeleteChannelMemberAsync(channelMember.MemberId, channelMember.ChannelId);

            var newChannelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(channelMember.MemberId, channelMember.ChannelId);

            newChannelMember.Should().BeNull();
        }

        [Fact]
        public async Task GetChannelMembersAsync_ShouldReturnAllChannelMembers()
        {
            var channelMembers = new List<ChannelMember>
            {
                new ChannelMember
                {
                    MemberId = _memberId,
                    ChannelId = _channelId,
                    LastReadMessageId = _lastReadMessageId,
                    IsMuted = true,
                    IsPinned = false
                },
                new ChannelMember
                {
                    MemberId = _secondMemberId,
                    ChannelId = _channelId,
                    IsMuted = false,
                    IsPinned = true
                }
            };

            foreach (var channelMember in channelMembers)
            {
                await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
            }

            var newChannelMembers = await UnitOfWork.ChannelMemberRepository.GetChannelMembersAsync(_channelId);

            newChannelMembers.Should().BeEquivalentTo(channelMembers);
        }

        [Fact]
        public async Task MuteChannelAsync_ShouldSetMuteField()
        {
            var channelMember = new ChannelMember
            {
                MemberId = _memberId,
                ChannelId = _channelId,
                IsMuted = false
            };
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);

            await UnitOfWork.ChannelMemberRepository.MuteChannelAsync(channelMember.MemberId, channelMember.ChannelId, true);
            var mutedMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(channelMember.MemberId, channelMember.ChannelId);
            mutedMember.IsMuted.Should().BeTrue();

            await UnitOfWork.ChannelMemberRepository.MuteChannelAsync(channelMember.MemberId, channelMember.ChannelId, false);
            var unmutedMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(channelMember.MemberId, channelMember.ChannelId);
            unmutedMember.IsMuted.Should().BeFalse();
        }

        [Fact]
        public async Task PinChannelAsync_ShouldChangeIsPinnedStatus()
        {
            var channelMember = new ChannelMember
            {
                MemberId = _memberId,
                ChannelId = _channelId,
                LastReadMessageId = null,
                IsPinned = false
            };
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);

            await UnitOfWork.ChannelMemberRepository.PinChannelAsync(_memberId, _channelId, true);
            var pinnedChannelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(_memberId, _channelId);
            pinnedChannelMember.IsPinned.Should().BeTrue();

            await UnitOfWork.ChannelMemberRepository.PinChannelAsync(_memberId, _channelId, false);
            var unPinnedChannelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(_memberId, _channelId);
            unPinnedChannelMember.IsPinned.Should().BeFalse();
        }

        [Fact]
        public async Task SetLastReadMessageAsync_ShouldSetLastReadMessage()
        {
            var channelMember = new ChannelMember
            {
                MemberId = _memberId,
                ChannelId = _channelId,
                LastReadMessageId = null
            };
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);

            await UnitOfWork.ChannelMemberRepository.SetLastReadMessageAsync(channelMember.MemberId, channelMember.ChannelId, _lastReadMessageId);

            var newChannelMember = await UnitOfWork.ChannelMemberRepository.GetChannelMemberAsync(channelMember.MemberId, channelMember.ChannelId);

            newChannelMember.LastReadMessageId.Should().Be(_lastReadMessageId);
        }

        [Fact]
        public async Task UpdateLastReadMessageAsync_ShouldUpdateLastReadMessageForAllChannelMembers()
        {
            // Arrange
            var currentLastReadMessage = new Message
            {
                Id = new Guid("08DB2602-E94F-4B8F-9061-E309912E1505"),
                Body = "message body",
                Created = DateTimeOffset.Now,
                ImageUrl = "test",
                Type = 0,
                ChannelId = _channelId,
                OwnerId = _memberId
            };
            await UnitOfWork.MessageRepository.AddMessageAsync(currentLastReadMessage);

            var channelMembers = new List<ChannelMember>
            {
                new ChannelMember
                {
                    MemberId = _memberId,
                    ChannelId = _channelId,
                    LastReadMessageId = _lastReadMessageId
                },
                new ChannelMember
                {
                    MemberId = _secondMemberId,
                    ChannelId = _channelId,
                    LastReadMessageId = _lastReadMessageId
                }
            };

            foreach (var channelMember in channelMembers)
            {
                await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
            }

            // Act
            await UnitOfWork.ChannelMemberRepository.UpdateLastReadMessageAsync(_lastReadMessageId, currentLastReadMessage.Id);

            // Assert
            var newChannelMembers = await UnitOfWork.ChannelMemberRepository.GetChannelMembersAsync(_channelId);
            foreach (var newChannelMember in newChannelMembers)
            {
                newChannelMember.LastReadMessageId.Should().Be(currentLastReadMessage.Id);
            }
        }

        [Fact]
        public async Task UpdateLastReadMessageAsync_ShouldSetLastReadMessageAsNULLForAllChannelMembers()
        {
            var channelMembers = new List<ChannelMember>
            {
                new ChannelMember
                {
                    MemberId = _memberId,
                    ChannelId = _channelId,
                    LastReadMessageId = _lastReadMessageId
                },
                new ChannelMember
                {
                    MemberId = _secondMemberId,
                    ChannelId = _channelId,
                    LastReadMessageId = _lastReadMessageId
                }
            };

            foreach (var channelMember in channelMembers)
            {
                await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
            }

            // Act
            await UnitOfWork.ChannelMemberRepository.UpdateLastReadMessageAsync(_lastReadMessageId, null);

            // Assert
            var newChannelMembers = await UnitOfWork.ChannelMemberRepository.GetChannelMembersAsync(_channelId);
            foreach (var newChannelMember in newChannelMembers)
            {
                newChannelMember.LastReadMessageId.Should().BeNull();
            }
        }
    }
}