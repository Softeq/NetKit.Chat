// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.DirectMembersService
{
    public class GetDirectChannelByIdTests : DirectMessagesTestBase
    {
        [Fact]
        public void ShouldThrowIfDirectMembersDoesNotExist()
        {
            // Arrange
            var id = new Guid("29251B3A-4D91-4664-A239-5EF3AED81FD6");

            _directChannelRepositoryMock.Setup(x => x.GetDirectChannelById(It.IsAny<Guid>()))
                .ReturnsAsync((DirectChannel)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await DirectMessageService.GetDirectChannelById(id); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get direct members. Chat with {nameof(id)}:{id} is not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfOwnerDoesNotExist()
        {
            // Arrange
            var id = new Guid("29251B3A-4D91-4664-A239-5EF3AED81FD6");
            var ownerId = new Guid("0D8BB89F-5A10-4E7C-8F49-73CE5B32DCD6");
            var memberId = new Guid("93F52D8D-0187-478B-9978-6A07D62A286C");

            var directMembers = new DirectChannel { Id = id, OwnerId = ownerId, MemberId = memberId };

            _directChannelRepositoryMock.Setup(x => x.GetDirectChannelById(It.Is<Guid>(d => d.Equals(id))))
                .ReturnsAsync(directMembers)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            //Act
            Func<Task> act = async () => { await DirectMessageService.GetDirectChannelById(id); };

            //Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get member {nameof(directMembers.OwnerId)}:{directMembers.OwnerId} is not found.");

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

            _directChannelRepositoryMock.Setup(x => x.GetDirectChannelById(It.Is<Guid>(d => d.Equals(directId))))
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

            var directMembersResponse = new DirectChannelResponse();
            _domainModelsMapperMock.Setup(x => x.MapToDirectChannelResponse(directChannel.Id, owner, member))
                .Returns(directMembersResponse)
                .Verifiable();

            // Act
            var act = await DirectMessageService.GetDirectChannelById(directId);

            // Assert
            act.Should().BeEquivalentTo(directMembersResponse);

            VerifyMocks();
        }
    }
}
