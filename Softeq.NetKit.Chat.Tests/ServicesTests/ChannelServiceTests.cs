// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Tests.Abstract;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.ServicesTests
{
    public class ChannelServiceTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("86f06c6c-39fd-414a-a9d5-b408c20734f7");
        private readonly Guid _memberId2 = new Guid("3e73bbaf-ab69-4d5d-bb8d-f5224f31473f");

        private const string SaasUserId = "2ae3a155-10b1-4c69-8573-d4527aba8860";
        private const string SaasUserId2 = "2dc14035-83e1-4d7b-9eec-0345474714d0";

        private readonly IChannelService _channelService;
        private readonly IMemberService _memberService;

        public ChannelServiceTests()
        {
            _channelService = LifetimeScope.Resolve<IChannelService>();
            _memberService = LifetimeScope.Resolve<IMemberService>();

            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Active,
                SaasUserId = SaasUserId
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task CreateChannelAsyncTest()
        {
            // Arrange
            var request = new CreateChannelRequest(SaasUserId, "name", ChannelType.Public)
            {
                Description = "test",
                WelcomeMessage = "test"
            };

            // Act
            var channel = await _channelService.CreateChannelAsync(request);
            var channelMessagesCount = await _channelService.GetChannelMessagesCountAsync(channel.Id);
            // Assert
            Assert.NotNull(channel);
            Assert.Equal(request.Name, channel.Name);
            Assert.Equal(request.Description, channel.Description);
            Assert.Equal(request.WelcomeMessage, channel.WelcomeMessage);
            Assert.Equal(request.Type, channel.Type);
            Assert.True(channelMessagesCount == 0);
        }

        [Fact]
        public async Task GetMyChannelsAsyncTest()
        {
            // Arrange
            var request = new CreateChannelRequest(SaasUserId, "name", ChannelType.Public)
            {
                Description = "test",
                WelcomeMessage = "test"
            };

            await _channelService.CreateChannelAsync(request);

            // Act
            var channels = await _channelService.GetMemberChannelsAsync(SaasUserId);

            // Assert
            Assert.NotNull(channels);
            Assert.NotEmpty(channels);
            Assert.Equal(request.Name, channels.First().Name);
            Assert.Equal(request.Description, channels.First().Description);
            Assert.Equal(request.WelcomeMessage, channels.First().WelcomeMessage);
            Assert.Equal(request.Type, channels.First().Type);
            Assert.Equal(_memberId, channels.First().CreatorId);
        }

        [Fact]
        public async Task UpdateChannelAsync()
        {
            // Arrange
            var request = new CreateChannelRequest(SaasUserId, "name", ChannelType.Public)
            {
                Description = "test",
                WelcomeMessage = "test"
            };

            var channel = await _channelService.CreateChannelAsync(request);

            var updatedRequest = new UpdateChannelRequest(SaasUserId, channel.Id, "test2")
            {
                Topic = "test2",
                WelcomeMessage = "test2"
            };

            // Act
            var updatedChannel = await _channelService.UpdateChannelAsync(updatedRequest);

            var channelMessagesCount = await _channelService.GetChannelMessagesCountAsync(channel.Id);

            // Assert
            Assert.NotNull(updatedChannel);
            Assert.NotNull(updatedChannel.Updated);
            Assert.Equal(updatedRequest.Name, updatedChannel.Name);
            Assert.Equal(updatedRequest.Topic, updatedChannel.Description);
            Assert.Equal(updatedRequest.WelcomeMessage, updatedChannel.WelcomeMessage);
            Assert.Equal(_memberId, updatedChannel.CreatorId);
            Assert.True(channelMessagesCount == 0);
            Assert.True(updatedChannel.MembersCount == 1);
        }

        [Fact]
        public async Task GetChannelByIdAsyncTest()
        {
            // Arrange
            var request = new CreateChannelRequest(SaasUserId, "name", ChannelType.Public)
            {
                Description = "test",
                WelcomeMessage = "test"
            };

            var channel = await _channelService.CreateChannelAsync(request);
            var channelMessagesCount = await _channelService.GetChannelMessagesCountAsync(channel.Id);

            // Act
            var newChannel = await _channelService.GetChannelByIdAsync(channel.Id);
            var channelMembers = await _memberService.GetChannelMembersAsync(channel.Id);

            // Assert
            Assert.NotNull(channelMembers);
            Assert.NotEmpty(channelMembers);
            Assert.NotNull(newChannel);
            Assert.Equal(request.Name, newChannel.Name);
            Assert.Equal(request.Description, newChannel.Description);
            Assert.Equal(request.WelcomeMessage, newChannel.WelcomeMessage);
            Assert.Equal(request.Type, newChannel.Type);
            Assert.True(channelMessagesCount == 0);
        }

        [Fact]
        public async Task CloseChannelAsyncTest()
        {
            // Arrange
            var request = new CreateChannelRequest(SaasUserId, "name", ChannelType.Public)
            {
                Description = "test",
                WelcomeMessage = "test"
            };

            var channel = await _channelService.CreateChannelAsync(request);

            // Act
            await _channelService.CloseChannelAsync(SaasUserId, channel.Id);
            var newChannel = await _channelService.GetChannelByIdAsync(channel.Id);

            // Assert
            Assert.NotNull(newChannel);
            Assert.True(newChannel.IsClosed);
        }

        [Fact]
        public async Task GetAllChannelsAsyncTest()
        {
            // Arrange
            var request = new CreateChannelRequest(SaasUserId, "name", ChannelType.Public)
            {
                Description = "test",
                WelcomeMessage = "test"
            };

            var channel = await _channelService.CreateChannelAsync(request);

            // Act
            var channels = await _channelService.GetAllChannelsAsync();
            var firstChannelId = channels.First().Id;
            var channelMessagesCount = await _channelService.GetChannelMessagesCountAsync(firstChannelId);

            // Assert
            Assert.NotNull(channels);
            Assert.NotEmpty(channels);
            Assert.Equal(request.Name, channels.First().Name);
            Assert.Equal(request.Description, channels.First().Description);
            Assert.Equal(request.WelcomeMessage, channels.First().WelcomeMessage);
            Assert.Equal(request.Type, channels.First().Type);
            Assert.Equal(_memberId, channels.First().CreatorId);
            Assert.True(channelMessagesCount == 0);
            Assert.True(channels.First().MembersCount == 1);
        }

        [Fact]
        public async Task GetChannelSettingsAsyncTest()
        {
            // Arrange
            var request = new CreateChannelRequest(SaasUserId, "name", ChannelType.Public)
            {
                Description = "test",
                WelcomeMessage = "test"
            };

            var channel = await _channelService.CreateChannelAsync(request);

            var settings = new Settings
            {
                Id = Guid.NewGuid(),
                ChannelId = channel.Id,
                RawSettings = "test"
            };

            await UnitOfWork.SettingRepository.AddSettingsAsync(settings);

            // Act
            var newSettings = await _channelService.GetChannelSettingsAsync(channel.Id);

            // Assert
            Assert.NotNull(newSettings);
            Assert.Equal(settings.Id, newSettings.Id);
            Assert.Equal(settings.ChannelId, newSettings.ChannelId);
            Assert.Equal(settings.RawSettings, newSettings.RawSettings);
        }

        [Fact]
        public async Task JoinToChannelAsyncTest()
        {
            // Arrange
            var request = new CreateChannelRequest(SaasUserId, "name", ChannelType.Public)
            {
                Description = "test",
                WelcomeMessage = "test"
            };

            var member = new Member
            {
                Id = _memberId2,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Active,
                SaasUserId = SaasUserId2
            };

            await UnitOfWork.MemberRepository.AddMemberAsync(member);
            var channel = await _channelService.CreateChannelAsync(request);
            
            // Act
            await _channelService.JoinToChannelAsync(SaasUserId2, channel.Id);
            var newChannel = await _channelService.GetChannelByIdAsync(channel.Id);
            var channelMembers = await _memberService.GetChannelMembersAsync(channel.Id);

            // Assert
            Assert.NotNull(newChannel);
            Assert.NotNull(channelMembers);
            Assert.NotEmpty(channelMembers);
            Assert.Equal(member.Id, channelMembers.First(x => x.Id == _memberId2).Id);
            Assert.Equal(member.LastActivity, channelMembers.First(x => x.Id == _memberId2).LastActivity);
            Assert.Equal(member.Status, channelMembers.First(x => x.Id == _memberId2).Status);
            Assert.Equal(member.SaasUserId, channelMembers.First(x => x.Id == _memberId2).SaasUserId);
        }

        [Fact]
        public async Task LeaveChannelAsyncTest()
        {
            // Arrange
            var request = new CreateChannelRequest(SaasUserId, "name", ChannelType.Public)
            {
                Description = "test",
                WelcomeMessage = "test"
            };

            var member = new Member
            {
                Id = _memberId2,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Active,
                SaasUserId = SaasUserId2
            };

            await UnitOfWork.MemberRepository.AddMemberAsync(member);
            var channel = await _channelService.CreateChannelAsync(request);
            await _channelService.JoinToChannelAsync(SaasUserId2, channel.Id);
            var previousMembersCount = (await _memberService.GetChannelMembersAsync(channel.Id)).Count;
            
            // Act
            await _channelService.LeaveFromChannelAsync(SaasUserId2, channel.Id);
            var newChannel = await _channelService.GetChannelByIdAsync(channel.Id);
            var channelMembers = await _memberService.GetChannelMembersAsync(channel.Id);

            // Assert
            Assert.NotNull(newChannel);
            Assert.NotNull(channelMembers);
        }

        [Fact]
        public async Task PinChannelAsync_ShouldChangeIsPinnedStatus()
        {
            var createChannelRequest = new CreateChannelRequest(SaasUserId, "name", ChannelType.Public)
            {
                Description = "test",
                WelcomeMessage = "test"
            };
            var channel = await _channelService.CreateChannelAsync(createChannelRequest);

            await _channelService.PinChannelAsync(SaasUserId, channel.Id, true);
            var pinnedChannel = await _channelService.GetChannelSummaryAsync(SaasUserId, channel.Id);

            pinnedChannel.IsPinned.Should().BeTrue();

            await _channelService.PinChannelAsync(SaasUserId, channel.Id, false);
            var unPinnedChannel = await _channelService.GetChannelSummaryAsync(SaasUserId, channel.Id);

            unPinnedChannel.IsPinned.Should().BeFalse();
        }
    }
}