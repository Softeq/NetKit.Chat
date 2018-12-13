// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelService
{
    public class CreateChannelAsyncTests : ChannelServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var request = new CreateChannelRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", "channel name", ChannelType.Public);

            // Act
            Func<Task> act = async () => { await _channelService.CreateChannelAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to create channel. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldCreatePublicChannelWithoutPhoto_AndShouldNotAddAllowedMembers()
        {
            // Arrange
            var request = new CreateChannelRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", "channel name", ChannelType.Public)
            {
                AllowedMembers = new List<string> { "A53F4EA4-B5BD-494A-8D5F-4B4B172A25F8" }
            };

            var member = new Member { Id = new Guid("85B1E28A-3E29-48C5-B85B-1563EEB60742") };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId => saasUserId.Equals(request.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _cloudImageProviderMock.Setup(x => x.CopyImageToDestinationContainerAsync(It.IsAny<string>()))
                .ReturnsAsync((string)null)
                .Verifiable();

            var utcNow = DateTimeOffset.UtcNow;
            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(utcNow)
                .Verifiable();

            Channel channelToAdd = null;
            _channelRepositoryMock.Setup(x => x.AddChannelAsync(It.IsAny<Channel>()))
                .Callback<Channel>(x => channelToAdd = x)
                .Returns(Task.CompletedTask)
                .Verifiable();

            ChannelMember channelMemberToAdd = null;
            _channelMemberRepositoryMock.Setup(x => x.AddChannelMemberAsync(It.IsAny<ChannelMember>()))
                .Callback<ChannelMember>(x => channelMemberToAdd = x)
                .Returns(Task.CompletedTask)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.IncrementChannelMembersCount(It.IsAny<Guid>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var addedChannel = new Channel();
            _channelRepositoryMock.Setup(x => x.GetChannelWithCreatorAsync(It.IsAny<Guid>()))
                .ReturnsAsync(addedChannel)
                .Verifiable();

            var channelSummaryResponse = new ChannelSummaryResponse();
            _domainModelsMapperMock.Setup(x => x.MapToChannelSummaryResponse(
                    It.Is<Channel>(channel => channel.Equals(addedChannel)),
                    It.IsAny<ChannelMember>(),
                    It.IsAny<Message>()))
                .Returns(channelSummaryResponse)
                .Verifiable();

            // Act
            var result = await _channelService.CreateChannelAsync(request);

            // Assert
            VerifyMocks();
            channelToAdd.Created.Should().Be(utcNow);
            channelToAdd.Name.Should().Be(request.Name);
            channelToAdd.Type.Should().Be(request.Type);
            channelToAdd.CreatorId.Should().Be(member.Id);
            channelToAdd.Creator.Should().Be(member);
            channelToAdd.PhotoUrl.Should().Be(null);
            channelToAdd.Members.Should().BeEquivalentTo(new List<ChannelMember> {channelMemberToAdd});

            _channelMemberRepositoryMock.Verify(prov => prov.AddChannelMemberAsync(It.IsAny<ChannelMember>()), Times.Once);
            _channelRepositoryMock.Verify(prov => prov.IncrementChannelMembersCount(It.Is<Guid>(channelId => channelId.Equals(channelToAdd.Id))), Times.Once);

            result.Should().BeEquivalentTo(channelSummaryResponse);
        }
    }
}