// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Settings;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelService
{
    public class GetChannelSettingsAsyncTests : ChannelServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfSettingDoesNotExist()
        {
            // Arrange
            var channelId = new Guid("BAFDCFAA-267D-4D16-98B0-6D516D9193ED");

            _settingRepositoryMock.Setup(x => x.GetSettingsByChannelIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Settings)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await _channelService.GetChannelSettingsAsync(channelId); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get channel settings. Settings with {nameof(channelId)}:{channelId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldReturnSettingsResponse()
        {
            // Arrange
            var channelId = new Guid("5537AB43-B861-40F4-A446-B74174489B23");
            var setting = new Settings();

            _settingRepositoryMock.Setup(x => x.GetSettingsByChannelIdAsync(It.Is<Guid>(ch => ch.Equals(channelId))))
                .ReturnsAsync(setting)
                .Verifiable();

            var settingsResponse = new SettingsResponse();
            _domainModelsMapperMock.Setup(x => x.MapToSettingsResponse(setting))
                .Returns(settingsResponse)
                .Verifiable();

            // Act
            var act = await _channelService.GetChannelSettingsAsync(channelId);

            // Assert
            act.Should().BeEquivalentTo(settingsResponse);
        }
    }
}
