// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.RepositoryTests
{
    public class ChannelRepositoryTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("c4d7e362-d49c-4b19-95e9-70cb169467bd");
        private readonly Guid _member2Id = new Guid("e173bacf-e17f-46fb-9a83-012c95776eb9");
        
        public ChannelRepositoryTests()
        {
            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Active
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            var member2 = new Member
            {
                Id = _member2Id,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Active
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member2).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task AddChannelAsync_ShouldAddChannel()
        {
            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Name = "Name",
                Type = ChannelType.Public,
                Description = "Description",
                WelcomeMessage = "WelcomeMessage",
                MembersCount = 10,
                Updated = DateTimeOffset.Now,
                PhotoUrl = "PhotoUrl"
            };

            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);

            var newChannel = await UnitOfWork.ChannelRepository.GetChannelAsync(channel.Id);

            newChannel.Should().BeEquivalentTo(channel);
        }

        [Fact]
        public async Task UpdateChannelAsync_ShouldUpdateChannel()
        {
            var channel = new Channel
            {
                Id = new Guid("CF2EA610-91F7-45C7-8700-9626216120A0"),
                IsClosed = false,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Name = "Name",
                Type = ChannelType.Public,
                Description = "Description",
                WelcomeMessage = "WelcomeMessage",
                MembersCount = 10,
                Updated = DateTimeOffset.Now,
                PhotoUrl = "PhotoUrl"
            };
            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);

            var channelToUpdate = new Channel
            {
                Id = channel.Id,
                IsClosed = true,
                CreatorId = channel.CreatorId,
                Created = channel.Created,
                Name = "Name2",
                Type = ChannelType.Private,
                Description = "Description2",
                WelcomeMessage = "WelcomeMessage2",
                MembersCount = 20,
                Updated = channel.Updated + TimeSpan.FromMinutes(1),
                PhotoUrl = "PhotoUrl2"
            };

            await UnitOfWork.ChannelRepository.UpdateChannelAsync(channelToUpdate);

            var updatedChannel = await UnitOfWork.ChannelRepository.GetChannelAsync(channelToUpdate.Id);

            updatedChannel.Should().BeEquivalentTo(channelToUpdate);
        }

        [Fact]
        public async Task GetAllChannelsAsync_ShouldReturnAllExistingChannels()
        {
            var channels = new List<Channel>
            {
                new Channel
                {
                    Id = Guid.NewGuid(),
                    IsClosed = true,
                    CreatorId = _memberId,
                    Created = DateTimeOffset.UtcNow,
                    Type = ChannelType.Public,
                    MembersCount = 1
                },
                new Channel
                {
                    Id = Guid.NewGuid(),
                    IsClosed = true,
                    CreatorId = _member2Id,
                    Created = DateTimeOffset.UtcNow,
                    Type = ChannelType.Private,
                    MembersCount = 2
                }
            };

            foreach (var channel in channels)
            {
                await UnitOfWork.ChannelRepository.AddChannelAsync(channel);
            }

            var newChannels = await UnitOfWork.ChannelRepository.GetAllChannelsAsync();

            newChannels.Should().BeEquivalentTo(channels);
        }

        [Fact]
        public async Task GetAllowedChannelsWithMessagesAndCreatorAsync_ShouldReturnAllOpenChannelsWhereMemberParticipateWithMessages()
        {
            // Arrange
            var member = new Member
            {
                Id = new Guid("78D80247-DD60-49AB-821C-069D47130DF9"),
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Active
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            var member2 = new Member
            {
                Id = new Guid("401907DC-802D-4673-BA84-74096369F958"),
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Active
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member2).GetAwaiter().GetResult();

            var participatedChannels = new List<Channel>
            {
                new Channel
                {
                    Id = Guid.NewGuid(),
                    IsClosed = false,
                    CreatorId = member.Id,
                    Creator = member,
                    Created = DateTimeOffset.UtcNow,
                    Type = ChannelType.Public,
                    MembersCount = 10
                },
                new Channel
                {
                    Id = Guid.NewGuid(),
                    IsClosed = false,
                    CreatorId = member2.Id,
                    Creator = member2,
                    Created = DateTimeOffset.UtcNow,
                    Type = ChannelType.Private,
                    MembersCount = 10
                }
            };
            foreach (var channel in participatedChannels)
            {
                await UnitOfWork.ChannelRepository.AddChannelAsync(channel);
                await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(new ChannelMember { MemberId = member.Id, ChannelId = channel.Id });

                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    Body = "Body",
                    Created = DateTimeOffset.Now,
                    ImageUrl = "ImageUrl",
                    Type = MessageType.Default,
                    ChannelId = channel.Id,
                    OwnerId = member.Id,
                    Owner = member
                };
                await UnitOfWork.MessageRepository.AddMessageAsync(message);
                channel.Messages = new List<Message> { message };
            }

            var closedChannel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = member.Id,
                Created = DateTimeOffset.UtcNow,
                Type = ChannelType.Public,
                MembersCount = 10
            };
            await UnitOfWork.ChannelRepository.AddChannelAsync(closedChannel);
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(new ChannelMember { MemberId = member.Id, ChannelId = closedChannel.Id });

            var notParticipatedChannel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = false,
                CreatorId = member2.Id,
                Created = DateTimeOffset.UtcNow,
                Type = ChannelType.Private,
                MembersCount = 10
            };
            await UnitOfWork.ChannelRepository.AddChannelAsync(notParticipatedChannel);
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(new ChannelMember { MemberId = member2.Id, ChannelId = notParticipatedChannel.Id });

            // Act
            var allowedChannels = await UnitOfWork.ChannelRepository.GetAllowedChannelsWithMessagesAndCreatorAsync(member.Id);

            // Assert
            allowedChannels.Should().BeEquivalentTo(participatedChannels);
        }

        [Fact]
        public async Task GetAllowedChannelsAsync_ShouldReturnAllOpenChannelsWhereMemberParticipate()
        {
            // Arrange
            var participatedChannels = new List<Channel>
            {
                new Channel
                {
                    Id = Guid.NewGuid(),
                    IsClosed = false,
                    CreatorId = _memberId,
                    Created = DateTimeOffset.UtcNow,
                    Type = ChannelType.Public,
                    MembersCount = 10
                },
                new Channel
                {
                    Id = Guid.NewGuid(),
                    IsClosed = false,
                    CreatorId = _member2Id,
                    Created = DateTimeOffset.UtcNow,
                    Type = ChannelType.Private,
                    MembersCount = 10
                }
            };
            foreach (var channel in participatedChannels)
            {
                await UnitOfWork.ChannelRepository.AddChannelAsync(channel);
                await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(new ChannelMember { MemberId = _memberId, ChannelId = channel.Id });
            }

            var closedChannel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Type = ChannelType.Public,
                MembersCount = 10
            };
            await UnitOfWork.ChannelRepository.AddChannelAsync(closedChannel);
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(new ChannelMember { MemberId = _memberId, ChannelId = closedChannel.Id });

            var notParticipatedChannel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = false,
                CreatorId = _member2Id,
                Created = DateTimeOffset.UtcNow,
                Type = ChannelType.Private,
                MembersCount = 10
            };
            await UnitOfWork.ChannelRepository.AddChannelAsync(notParticipatedChannel);
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(new ChannelMember { MemberId = _member2Id, ChannelId = notParticipatedChannel.Id });

            // Act
            var allowedChannels = await UnitOfWork.ChannelRepository.GetAllowedChannelsAsync(_memberId);

            // Assert
            allowedChannels.Should().BeEquivalentTo(participatedChannels);
        }

        [Fact]
        public async Task IsChannelExistsAsync_ShouldReturnTrueIfChannelExists()
        {
            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = false,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Type = ChannelType.Public,
                MembersCount = 10
            };

            var exists = await UnitOfWork.ChannelRepository.IsChannelExistsAsync(channel.Id);
            exists.Should().BeFalse();

            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);

            exists = await UnitOfWork.ChannelRepository.IsChannelExistsAsync(channel.Id);
            exists.Should().BeTrue();
        }
        
        [Fact]
        public async Task GetChannelAsync_ShouldReturnExistingChannel()
        {
            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Name = "Name",
                Type = ChannelType.Public,
                Description = "Description",
                WelcomeMessage = "WelcomeMessage",
                MembersCount = 10,
                PhotoUrl = "PhotoUrl",
                Updated = DateTimeOffset.Now
            };

            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);

            var newChannel = await UnitOfWork.ChannelRepository.GetChannelAsync(channel.Id);

            newChannel.Should().BeEquivalentTo(channel);
        }

        [Fact]
        public async Task GetChannelWithCreatorAsync_ShouldReturnExistingChannelWithCreatorInformation()
        {
            var creator = new Member
            {
                Id = new Guid("9B6C7A69-85B2-4F43-A08B-ABC4879C00E4"),
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Offline,
                SaasUserId = "SaasUserId",
                Name = "Name",
                Role = UserRole.User,
                IsAfk = true,
                Email = "Email"
            };

            await UnitOfWork.MemberRepository.AddMemberAsync(creator);

            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = creator.Id,
                Created = DateTimeOffset.UtcNow,
                Name = "Name",
                Type = ChannelType.Public,
                Description = "Description",
                WelcomeMessage = "WelcomeMessage",
                MembersCount = 10,
                PhotoUrl = "PhotoUrl",
                Updated = DateTimeOffset.Now,
                Creator = creator
            };

            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);

            var newChannel = await UnitOfWork.ChannelRepository.GetChannelWithCreatorAsync(channel.Id);

            newChannel.Should().BeEquivalentTo(channel);
        }

        [Fact]
        public async Task IsMemberExistsInChannelAsync_ShouldReturnTrueIfExistsAndFalseIfNot()
        {
            // Arrange
            var creatorAndParticipate = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Type = ChannelType.Public,
                MembersCount = 10
            };
            await UnitOfWork.ChannelRepository.AddChannelAsync(creatorAndParticipate);
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(new ChannelMember { MemberId = _memberId, ChannelId = creatorAndParticipate.Id });

            var notCreatorAndParticipate = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _member2Id,
                Created = DateTimeOffset.UtcNow,
                Type = ChannelType.Public,
                MembersCount = 10
            };
            await UnitOfWork.ChannelRepository.AddChannelAsync(notCreatorAndParticipate);
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(new ChannelMember { MemberId = _memberId, ChannelId = notCreatorAndParticipate.Id });

            var creatorAndNotParticipate = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Type = ChannelType.Public,
                MembersCount = 10
            };
            await UnitOfWork.ChannelRepository.AddChannelAsync(creatorAndNotParticipate);

            // Act
            var whenCreatorAndParticipate = await UnitOfWork.ChannelRepository.IsMemberExistsInChannelAsync(_memberId, creatorAndParticipate.Id);
            var whenNotCreatorAndParticipate = await UnitOfWork.ChannelRepository.IsMemberExistsInChannelAsync(_memberId, notCreatorAndParticipate.Id);
            var whenCreatorAndNotParticipate = await UnitOfWork.ChannelRepository.IsMemberExistsInChannelAsync(_memberId, creatorAndNotParticipate.Id);

            // Assert
            whenCreatorAndParticipate.Should().BeTrue();
            whenNotCreatorAndParticipate.Should().BeTrue();
            whenCreatorAndNotParticipate.Should().BeFalse();
        }

        [Fact]
        public async Task IncrementChannelMembersCount_ShouldIncrementMembersCount()
        {
            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Type = ChannelType.Public,
                MembersCount = 0
            };
            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);

            await UnitOfWork.ChannelRepository.IncrementChannelMembersCount(channel.Id);

            var incrementedChannel = await UnitOfWork.ChannelRepository.GetChannelAsync(channel.Id);

            incrementedChannel.MembersCount.Should().Be(channel.MembersCount + 1);
        }

        [Fact]
        public async Task DecrementChannelMembersCount_ShouldDecrementMembersCount()
        {
            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Type = ChannelType.Public,
                MembersCount = 1
            };
            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);

            await UnitOfWork.ChannelRepository.DecrementChannelMembersCount(channel.Id);

            var incrementedChannel = await UnitOfWork.ChannelRepository.GetChannelAsync(channel.Id);

            incrementedChannel.MembersCount.Should().Be(channel.MembersCount - 1);
        }
    }
}