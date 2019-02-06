﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.DirectChannelService
{
    public class GetDirectChannelByIdTests : DirectChannelTestBase
    {
        [Fact]
        public void ShouldThrowIfDirectChannelDoesNotExist()
        {
            // Arrange
            var id = new Guid("29251B3A-4D91-4664-A239-5EF3AED81FD6");

            _directChannelRepositoryMock.Setup(x => x.GetDirectChannelAsync(It.IsAny<Guid>()))
                .ReturnsAsync((DirectChannel)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await DirectChannelService.GetDirectChannelByIdAsync(id); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get direct channel. Chat with channelId:{id} is not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfOwnerDoesNotExist()
        {
            // Arrange
            var id = new Guid("29251B3A-4D91-4664-A239-5EF3AED81FD6");
            var ownerId = new Guid("0D8BB89F-5A10-4E7C-8F49-73CE5B32DCD6");
            var memberId = new Guid("93F52D8D-0187-478B-9978-6A07D62A286C");

            var directChannel = new DirectChannel { Id = id, OwnerId = ownerId, MemberId = memberId };

            _directChannelRepositoryMock.Setup(x => x.GetDirectChannelAsync(It.Is<Guid>(d => d.Equals(id))))
                .ReturnsAsync(directChannel)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            //Act
            Func<Task> act = async () => { await DirectChannelService.GetDirectChannelByIdAsync(id); };

            //Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get member {nameof(directChannel.OwnerId)}:{directChannel.OwnerId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var directId = new Guid("29251B3A-4D91-4664-A239-5EF3AED81FD6");
            var ownerId = new Guid("0D8BB89F-5A10-4E7C-8F49-73CE5B32DCD6");
            var memberId = new Guid("93F52D8D-0187-478B-9978-6A07D62A286C");

            var directChannel = new DirectChannel { Id = directId, OwnerId = ownerId, MemberId = memberId };

            _directChannelRepositoryMock.Setup(x => x.GetDirectChannelAsync(It.Is<Guid>(d => d.Equals(directId))))
                .ReturnsAsync(directChannel)
                .Verifiable();

            var owner = new Member { Id = ownerId };
            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.Is<Guid>(id => id.Equals(directChannel.OwnerId))))
                         .ReturnsAsync(owner)
                         .Verifiable();

            var member = new Member { Id = memberId };
            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.Is<Guid>(id => id.Equals(directChannel.MemberId))))
                .ReturnsAsync(member)
                .Verifiable();

            var channelResponse = new DirectChannelResponse();
            _domainModelsMapperMock.Setup(x => x.MapToDirectChannelResponse(directChannel.Id, owner, member))
                .Returns(channelResponse)
                .Verifiable();

            // Act
            var act = await DirectChannelService.GetDirectChannelByIdAsync(directId);

            // Assert
            act.Should().BeEquivalentTo(channelResponse);

            VerifyMocks();
        }
    }
}
