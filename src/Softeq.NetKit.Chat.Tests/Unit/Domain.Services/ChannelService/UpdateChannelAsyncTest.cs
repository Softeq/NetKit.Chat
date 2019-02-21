// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelService
{
    public class UpdateChannelAsyncTest : ChannelServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void NoChannel_ShouldThrowIfChannelDoesNotExist()
        {
            // Arrange
            var request = new UpdateChannelRequest("F9253C29-F09C-4B67-AF7F-E600BC153FD3", new Guid("60621580-7C10-48F0-B7F2-734735AD4A7C"), "name");
            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Channel)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _channelService.UpdateChannelAsync(request);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to update channel. Channel {nameof(request.ChannelId)}:{request.ChannelId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void NotEqualsMemberIdAndChannelCreatorId_ShouldThrowIfMemberIsNotChannelCreator()
        {
            // Arrange
            var request = new UpdateChannelRequest("F9253C29-F09C-4B67-AF7F-E600BC153FD3", new Guid("60621580-7C10-48F0-B7F2-734735AD4A7C"), "name");

            var channel = new Channel { CreatorId = request.ChannelId };

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.Is<Guid>(id => id.Equals(request.ChannelId))))
                .ReturnsAsync(channel)
                .Verifiable();

            var member = new Member { Id = new Guid("14C8640C-9D55-47D9-8D0B-D726703CFBE9") };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(member)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _channelService.UpdateChannelAsync(request);

            // Assert
            act.Should().Throw<NetKitChatAccessForbiddenException>().And.Message.Should()
                .Be($"Unable to update channel { nameof(request.ChannelId)}:{ request.ChannelId}. Channel owner required.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldUpdateChannel()
        {
            // Arrange
            var request = new UpdateChannelRequest("F9253C29-F09C-4B67-AF7F-E600BC153FD3", new Guid("A2BE089F-696C-4097-80EA-ABDAF31D098D"), "name")
            {
                Description = "Description",
                WelcomeMessage = "Welcome Message",
                PhotoUrl = "PhotoUrl"
            };

            var channelCreator = new Member { Id = new Guid("137FE248-7BE7-46A1-8D4D-258AD4A1418D") };

            var channel = new Channel { CreatorId = channelCreator.Id };

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.Is<Guid>(id => id.Equals(request.ChannelId))))
                .ReturnsAsync(channel)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId => saasUserId.Equals(request.SaasUserId))))
                .ReturnsAsync(channelCreator)
                .Verifiable();

            const string cloudPhotoUrl = "cloudPhotoUrl";
            _cloudImageProviderMock.Setup(x => x.CopyImageToDestinationContainerAsync(It.Is<string>(photoUrl => photoUrl.Equals(request.PhotoUrl))))
                .ReturnsAsync(cloudPhotoUrl)
                .Verifiable();

            var utcNow = DateTimeOffset.UtcNow;
            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(utcNow)
                .Verifiable();

            Channel channelToUpdate = null;
            _channelRepositoryMock.Setup(x => x.UpdateChannelAsync(It.Is<Channel>(c => c.Equals(channel))))
                .Callback<Channel>(x => channelToUpdate = x)
                .Returns(Task.CompletedTask)
                .Verifiable();

            var mappedChannel = new ChannelResponse();
            _domainModelsMapperMock.Setup(x => x.MapToChannelResponse(It.Is<Channel>(c => c.Equals(channel))))
                .Returns(mappedChannel)
                .Verifiable();

            // Act
            var result = await _channelService.UpdateChannelAsync(request);

            // Assert
            VerifyMocks();

            channelToUpdate.Description.Should().BeEquivalentTo(request.Description);
            channelToUpdate.WelcomeMessage.Should().BeEquivalentTo(request.WelcomeMessage);
            channelToUpdate.Name.Should().BeEquivalentTo(request.Name);
            channelToUpdate.PhotoUrl.Should().BeEquivalentTo(cloudPhotoUrl);
            channelToUpdate.Updated.Should().Be(utcNow);

            result.Should().BeEquivalentTo(mappedChannel);
        }
    }
}
