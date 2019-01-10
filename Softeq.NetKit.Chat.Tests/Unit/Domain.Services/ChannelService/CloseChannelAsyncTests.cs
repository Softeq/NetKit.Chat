// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.ChannelService
{
    public class CloseChannelAsyncTests : ChannelServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfChannelDoesNotExist()
        {
            // Arrange
            var saasUserId = "A0045650-27E0-4454-8720-A195DE9A96FF";
            var channelId = new Guid("8698C83C-FCDC-4920-8CA7-6D4EDBB14971");

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Channel)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _channelService.CloseChannelAsync(saasUserId, channelId);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to close channel. Channel {nameof(channelId)}:{channelId} not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfChannelAlreadyClosed()
        {
            // Arrange
            var saasUserId = "A0045650-27E0-4454-8720-A195DE9A96FF";
            var channelId = new Guid("8698C83C-FCDC-4920-8CA7-6D4EDBB14971");

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Channel { IsClosed = true })
                .Verifiable();

            // Act
            Func<Task> act = async () => await _channelService.CloseChannelAsync(saasUserId, channelId);

            // Assert
            act.Should().Throw<NetKitChatInvalidOperationException>().And.Message.Should()
                .Be($"Unable to close channel. Channel {nameof(channelId)}:{channelId} already closed.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfMemberNotExists()
        {
            // Arrange
            var saasUserId = "BC6CDEBF-6D6A-467C-98EF-133DBA26EAD4";
            var channelId = new Guid("7A5006DC-B5DC-4D98-AE4B-D4518D149A5A");

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Channel { IsClosed = false })
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _channelService.CloseChannelAsync(saasUserId, channelId);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to close channel. Member {nameof(saasUserId)}:{saasUserId} not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfMemberIdNotEqualsChannelCreatorId()
        {
            // Arrange
            var saasUserId = "BAFDCFAA-267D-4D16-98B0-6D516D9193ED";
            var channelId = new Guid("7A5006DC-B5DC-4D98-AE4B-D4518D149A5A");

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Channel { CreatorId = new Guid() })
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new Member { Id = new Guid("8F5A162D-2A47-4871-9608-9BFF627DFC72") })
                .Verifiable();

            // Act
            Func<Task> act = async () => await _channelService.CloseChannelAsync(saasUserId, channelId);

            // Assert
            act.Should().Throw<NetKitChatAccessForbiddenException>().And.Message.Should()
                .Be($"Unable to close channel {nameof(channelId)}:{channelId}. Channel owner required.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldReturnChannelResponse()
        {
            // Arrange
            var saasUserId = "BAFDCFAA-267D-4D16-98B0-6D516D9193ED";
            var channel = new Channel
            {
                Id = new Guid("7A5006DC-B5DC-4D98-AE4B-D4518D149A5A"),
                CreatorId = new Guid("8F5A162D-2A47-4871-9608-9BFF627DFC72"),
                IsClosed = false
            };

            var member = new Member { Id = new Guid("8F5A162D-2A47-4871-9608-9BFF627DFC72") };

            _channelRepositoryMock.Setup(x => x.GetChannelAsync(It.Is<Guid>(idChannel => idChannel.Equals(channel.Id))))
                .ReturnsAsync(channel)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasId => saasId.Equals(saasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _channelRepositoryMock.Setup(x => x.UpdateChannelAsync(It.Is<Channel>(ch => ch.Equals(channel)))).Returns(Task.CompletedTask);

            var mappedChannel = new ChannelResponse();
            _domainModelsMapperMock.Setup(x => x.MapToChannelResponse(It.Is<Channel>(c => c.Equals(channel))))
                .Returns(mappedChannel)
                .Verifiable();

            // Act
            var act = await _channelService.CloseChannelAsync(saasUserId, channel.Id);

            // Assert
            act.Should().BeEquivalentTo(mappedChannel);

            VerifyMocks();
        }
    }
}
