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
    public class SettingRepositoryTests : BaseTest
    {
        private readonly Guid _channelId = new Guid("FE711AF3-DDE7-4B11-BB9B-11C3862262EE");

        public SettingRepositoryTests()
        {
            var member = new Member
            {
                Id = new Guid("FE711AF3-DDE7-4B11-BB9B-55C3862262AA"),
                LastActivity = DateTimeOffset.UtcNow,
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
        public async Task AddSettingsAsync_ShouldAddSettings()
        {
            var settings = new Settings
            {
                Id = Guid.NewGuid(),
                RawSettings = "RawSettings",
                ChannelId = _channelId
            };

            await UnitOfWork.SettingRepository.AddSettingsAsync(settings);

            var newSettings = await UnitOfWork.SettingRepository.GetSettingsByIdAsync(settings.Id);

            newSettings.Should().BeEquivalentTo(settings);
        }

        [Fact]
        public async Task DeleteSettingsAsync_ShouldDeleteSettings()
        {
            var settings = new Settings
            {
                Id = Guid.NewGuid(),
                RawSettings = "RawSettings",
                ChannelId = _channelId
            };
            
            await UnitOfWork.SettingRepository.AddSettingsAsync(settings);

            await UnitOfWork.SettingRepository.DeleteSettingsAsync(settings.Id);

            var deletedSettings = await UnitOfWork.SettingRepository.GetSettingsByIdAsync(settings.Id);

            deletedSettings.Should().BeNull();
        }

        [Fact]
        public async Task GetSettingsByIdAsync_ShouldReturnSettings()
        {
            var settings = new Settings
            {
                Id = Guid.NewGuid(),
                RawSettings = "RawSettings",
                ChannelId = _channelId
            };
            
            await UnitOfWork.SettingRepository.AddSettingsAsync(settings);

            var newSettings = await UnitOfWork.SettingRepository.GetSettingsByIdAsync(settings.Id);

            newSettings.Should().BeEquivalentTo(settings);
        }

        [Fact]
        public async Task GetSettingsByChannelIdAsync_ShouldReturnSettings()
        {
            var settings = new Settings
            {
                Id = Guid.NewGuid(),
                RawSettings = "RawSettings",
                ChannelId = _channelId
            };

            await UnitOfWork.SettingRepository.AddSettingsAsync(settings);

            var newSettings = await UnitOfWork.SettingRepository.GetSettingsByChannelIdAsync(_channelId);

            newSettings.Should().BeEquivalentTo(settings);
        }

        [Fact]
        public async Task GetAllSettingsAsync_ShouldReturnAllExistingSettings()
        {
            var expectedSettings = new List<Settings>();
            for (var i = 0; i < 3; i++)
            {
                var member = new Member
                {
                    Id = Guid.NewGuid(),
                    LastActivity = DateTimeOffset.UtcNow,
                    Status = UserStatus.Active
                };
                await UnitOfWork.MemberRepository.AddMemberAsync(member);

                var channel = new Channel
                {
                    Id = Guid.NewGuid(),
                    CreatorId = member.Id,
                    Name = "testSettingsChannel",
                    Type = ChannelType.Public,
                    MembersCount = 0
                };
                await UnitOfWork.ChannelRepository.AddChannelAsync(channel);

                var setting = new Settings
                {
                    Id = Guid.NewGuid(),
                    RawSettings = "RawSettings",
                    ChannelId = channel.Id
                };
                await UnitOfWork.SettingRepository.AddSettingsAsync(setting);
                expectedSettings.Add(setting);
            }
            
            var settings = await UnitOfWork.SettingRepository.GetAllSettingsAsync();

            settings.Should().BeEquivalentTo(expectedSettings);
        }
    }
}