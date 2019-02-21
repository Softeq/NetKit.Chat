using System;
using System.Threading.Tasks;
using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.RepositoryTests
{
    public class NotificationSettingsRepositoryTests : BaseTest
    {
        private readonly Guid _channelId = new Guid("FE711AF3-DDE7-4B11-BB9B-11C3862262EE");
        private readonly Guid _memberId = new Guid("FE711AF3-DDE7-4B11-BB9B-55C3862262AA");

        public NotificationSettingsRepositoryTests()
        {
            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            var channel = new Channel
            {
                Id = _channelId,
                CreatorId = member.Id,
                Name = "testSettingsChannel",
                Type = ChannelType.Public,
                MembersCount = 0
            };
            UnitOfWork.ChannelRepository.AddChannelAsync(channel).GetAwaiter().GetResult();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AddNotificationSettingsAsync_ShouldAddNotificationSettings()
        {
            var notificationSettings = new NotificationSettings
            {
                Id = Guid.NewGuid(),
                MemberId = _memberId
            };
            await UnitOfWork.NotificationSettingRepository.AddSettingsAsync(notificationSettings);
            var newSettings = await UnitOfWork.NotificationSettingRepository.GetSettingsByMemberIdAsync(_memberId);

            newSettings.Should().BeEquivalentTo(notificationSettings);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task UpdateNotificationSettingsAsync_ShouldUpdateNotificationSettings()
        {
            var notificationSettings = new NotificationSettings
            {
                Id = Guid.NewGuid(),
                MemberId = _memberId,
                IsChannelNotificationsDisabled = NotificationSettingValue.Enabled
            };

            await UnitOfWork.NotificationSettingRepository.AddSettingsAsync(notificationSettings);
            var newSettings = await UnitOfWork.NotificationSettingRepository.GetSettingsByMemberIdAsync(notificationSettings.MemberId);
            newSettings.Should().BeEquivalentTo(notificationSettings);

            notificationSettings.IsChannelNotificationsDisabled = NotificationSettingValue.Disabled;

            await UnitOfWork.NotificationSettingRepository.UpdateSettingsAsync(notificationSettings);
            var updatedSettings = await UnitOfWork.NotificationSettingRepository.GetSettingsByMemberIdAsync(notificationSettings.MemberId);
            updatedSettings.Should().BeEquivalentTo(notificationSettings);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetMemberIdsWithDisabledChannelNotificationsAsync_ShouldGetMemberIdsWithDisabledChannelNotifications()
        {
            var member2Id = Guid.NewGuid();
            var member3Id = Guid.NewGuid();

            var member2 = new Member
            {
                Id = member2Id,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online
            };
            var member3 = new Member
            {
                Id = member3Id,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online
            };

            await UnitOfWork.MemberRepository.AddMemberAsync(member2);
            await UnitOfWork.MemberRepository.AddMemberAsync(member3);

            var notificationSettings1 = new NotificationSettings
            {
                Id = Guid.NewGuid(),
                MemberId = _memberId,
                IsChannelNotificationsDisabled = NotificationSettingValue.Disabled
            };
            var notificationSettings2 = new NotificationSettings
            {
                Id = Guid.NewGuid(),
                MemberId = member2Id,
                IsChannelNotificationsDisabled = NotificationSettingValue.Disabled
            };
            var notificationSettings3 = new NotificationSettings
            {
                Id = Guid.NewGuid(),
                MemberId = member3Id
            };

            await UnitOfWork.NotificationSettingRepository.AddSettingsAsync(notificationSettings1);
            await UnitOfWork.NotificationSettingRepository.AddSettingsAsync(notificationSettings2);
            await UnitOfWork.NotificationSettingRepository.AddSettingsAsync(notificationSettings3);

            var memberIds = await UnitOfWork.NotificationSettingRepository.GetSaasUserIdsWithDisabledGroupNotificationsAsync();

            memberIds.Count.Should().Be(2);
        }
    }
}
