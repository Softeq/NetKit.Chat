// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Tests.Abstract;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.RepositoryTests
{
    public class ChannelRepositoryTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("c4d7e362-d49c-4b19-95e9-70cb169467bd");
        private readonly Guid _memberId2 = new Guid("e173bacf-e17f-46fb-9a83-012c95776eb9");
        
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
                Id = _memberId2,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Active
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member2).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task AddChannelAsyncTest()
        {
            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Name = "test",
                Type = ChannelType.Public,
                Description = "test",
                WelcomeMessage = "test",
                MembersCount = 10
            };

            // Act
            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);
            var channelByName = await UnitOfWork.ChannelRepository.GetChannelByNameAsync(channel.Name);
            var channelById = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(channel.Id);

            // Assert
            Assert.NotNull(channelByName);
            AssertChannelEqualforAddChanelAsyncTest(channel, channelByName);
            AssertChannelEqualforAddChanelAsyncTest(channel, channelById);
        }

        [Fact]
        public async Task DeleteChannelAsyncTest()
        {
            // Arrange
            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Name = "test",
                Type = ChannelType.Public,
                Description = "test",
                WelcomeMessage = "test",
                MembersCount = 10
            };

            // Act
            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);
            await UnitOfWork.ChannelRepository.DeleteChannelAsync(channel.Id);
            var newChannel = await UnitOfWork.ChannelRepository.GetChannelByNameAsync(channel.Name);

            // Assert
            Assert.Null(newChannel);
        }

        [Fact]
        public async Task GetChannelByNameAsyncTest()
        {
            // Arrange
            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Name = "test",
                Type = ChannelType.Public,
                Description = "test",
                WelcomeMessage = "test",
                MembersCount = 10
            };

            // Act
            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);
            var newChannel = await UnitOfWork.ChannelRepository.GetChannelByNameAsync(channel.Name);

            // Assert
            Assert.NotNull(newChannel);
            Assert.Equal(channel.Id, newChannel.Id);
            Assert.Equal(channel.IsClosed, newChannel.IsClosed);
            Assert.Equal(channel.CreatorId, newChannel.CreatorId);
            Assert.Equal(channel.Name, newChannel.Name);
            Assert.Equal(channel.Type, newChannel.Type);
            Assert.Equal(channel.Description, newChannel.Description);
            Assert.Equal(channel.WelcomeMessage, newChannel.WelcomeMessage);
        }

        [Fact]
        public async Task GetAllChannelsAsyncTest()
        {
            // Arrange
            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Name = "test",
                Type = ChannelType.Public,
                Description = "test",
                WelcomeMessage = "test",
                MembersCount = 10
            };

            // Act
            var channels = await UnitOfWork.ChannelRepository.GetAllChannelsAsync();
            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);
            var newChannels = await UnitOfWork.ChannelRepository.GetAllChannelsAsync();

            // Assert
            Assert.NotNull(channels);
            Assert.Empty(channels);
            Assert.NotNull(newChannels);
            Assert.NotEmpty(newChannels);
            Assert.True(newChannels.Count > channels.Count);
        }

        [Fact]
        public async Task GetAllowedChannelsAsyncTest()
        {
            // Arrange
            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = false,
                CreatorId = _memberId2,
                Created = DateTimeOffset.UtcNow,
                Name = "test",
                Type = ChannelType.Public,
                Description = "test",
                WelcomeMessage = "test",
                MembersCount = 10
            };

            var channel2 = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = false,
                CreatorId = _memberId2,
                Created = DateTimeOffset.UtcNow,
                Name = "test2",
                Type = ChannelType.Public,
                Description = "test",
                WelcomeMessage = "test",
                MembersCount = 10
            };
            
            var allowedChannels = await UnitOfWork.ChannelRepository.GetAllowedChannelsAsync(_memberId2);
            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(new ChannelMembers
                {MemberId = _memberId, ChannelId = channel.Id});

            await UnitOfWork.ChannelRepository.AddChannelAsync(channel2);
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(new ChannelMembers
                { MemberId = _memberId2, ChannelId = channel2.Id });

            // Act
            var newAllowedChannels = await UnitOfWork.ChannelRepository.GetAllowedChannelsAsync(_memberId2);

            // Assert
            Assert.NotNull(allowedChannels);
            Assert.Empty(allowedChannels);
            Assert.NotNull(newAllowedChannels);
            Assert.NotEmpty(newAllowedChannels);
            Assert.True(newAllowedChannels.Count > allowedChannels.Count);
            Assert.True(newAllowedChannels.Count == 1);
        }

        [Fact]
        public async Task CheckIfUserExistInChannelAsyncTest()
        {
            // Arrange
            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId,
                Created = DateTimeOffset.UtcNow,
                Name = "test",
                Type = ChannelType.Public,
                Description = "test",
                WelcomeMessage = "test",
                MembersCount = 10
            };

            var channel2 = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = _memberId2,
                Created = DateTimeOffset.UtcNow,
                Name = "test2",
                Type = ChannelType.Public,
                Description = "test",
                WelcomeMessage = "test",
                MembersCount = 10
            };

            var channelMember = new ChannelMembers
            {
                ChannelId = channel.Id,
                MemberId = _memberId,
                LastReadMessageId = null
            };


            // Act           
            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);
            await UnitOfWork.ChannelRepository.AddChannelAsync(channel2);
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);

            var ensureExist = await UnitOfWork.ChannelRepository.IsMemberExistsInChannelAsync(_memberId, channel.Id);
            var ensureNonExist = await UnitOfWork.ChannelRepository.IsMemberExistsInChannelAsync(_memberId, channel2.Id);

            // Assert
            Assert.True(ensureExist);
            Assert.False(ensureNonExist);
        }

        #region Private methods
        
        private void AssertChannelEqualforAddChanelAsyncTest(Channel expectedChannel, Channel actualChannel)
        {
            Assert.Equal(expectedChannel.Id, actualChannel.Id);
            Assert.Equal(expectedChannel.IsClosed, actualChannel.IsClosed);
            Assert.Equal(expectedChannel.CreatorId, actualChannel.CreatorId);
            Assert.Equal(expectedChannel.Name, actualChannel.Name);
            Assert.Equal(expectedChannel.Type, actualChannel.Type);
            Assert.Equal(expectedChannel.Description, actualChannel.Description);
            Assert.Equal(expectedChannel.WelcomeMessage, actualChannel.WelcomeMessage);
        }

        #endregion
    }
}