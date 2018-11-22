// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Tests.Abstract;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.RepositoryTests
{
    public class SettingRepositoryTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("FE711AF3-DDE7-4B11-BB9B-55C3862262AA");
        private readonly Guid _channelId = new Guid("FE711AF3-DDE7-4B11-BB9B-11C3862262EE");

        public SettingRepositoryTests()
        {
            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTime.UtcNow,
                Status = UserStatus.Active
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
        public async Task AddSettingsAsyncTest()
        {
            var settings = new Settings
            {
                Id = Guid.NewGuid(),
                RawSettings = "test",
                ChannelId = _channelId
            };

            // Act
            await UnitOfWork.SettingRepository.AddSettingsAsync(settings);
            var newSettings = await UnitOfWork.SettingRepository.GetSettingsByIdAsync(settings.Id);

            // Assert
            Assert.NotNull(newSettings);
            Assert.Equal(settings.Id, newSettings.Id);
            Assert.Equal(settings.RawSettings, newSettings.RawSettings);
        }

        [Fact]
        public async Task DeleteSettingsAsyncTest()
        {
            // Arrange
            var settings = new Settings
            {
                Id = Guid.NewGuid(),
                RawSettings = "test",
                ChannelId = _channelId
            };

            // Act
            await UnitOfWork.SettingRepository.AddSettingsAsync(settings);
            await UnitOfWork.SettingRepository.DeleteSettingsAsync(settings.Id);
            var newSettings = await UnitOfWork.SettingRepository.GetSettingsByIdAsync(settings.Id);

            // Assert
            Assert.Null(newSettings);
        }

        [Fact]
        public async Task GetSettingsByIdAsyncTest()
        {
            // Arrange
            var settings = new Settings
            {
                Id = Guid.NewGuid(),
                RawSettings = "test",
                ChannelId = _channelId
            };

            // Act
            await UnitOfWork.SettingRepository.AddSettingsAsync(settings);
            var newSettings = await UnitOfWork.SettingRepository.GetSettingsByIdAsync(settings.Id);

            // Assert
            Assert.NotNull(newSettings);
            Assert.Equal(settings.Id, newSettings.Id);
            Assert.Equal(settings.RawSettings, newSettings.RawSettings);
        }

        [Fact]
        public async Task GetAllSettingsAsyncTest()
        {
            // Arrange
            var setting = new Settings
            {
                Id = Guid.NewGuid(),
                RawSettings = "test",
                ChannelId = _channelId
            };

            // Act
            var settings = await UnitOfWork.SettingRepository.GetAllSettingsAsync();
            await UnitOfWork.SettingRepository.AddSettingsAsync(setting);
            var newSettings = await UnitOfWork.SettingRepository.GetAllSettingsAsync();

            // Assert
            Assert.NotNull(settings);
            Assert.Empty(settings);
            Assert.NotNull(newSettings);
            Assert.NotEmpty(newSettings);
            Assert.True(newSettings.Count > settings.Count);
        }
    }
}