// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.DirectChannelService
{
    public class AddMessageAsyncTests : DirectChannelTestBase
    {
        [Fact]
        public void ShouldThrowIfChannelDoesNotExist()
        {
            // Arrange
            _directChannelRepositoryMock.Setup(x => x.GetDirectChannelById(It.IsAny<Guid>()))
                .ReturnsAsync((DirectChannel)null)
                .Verifiable();

            var directMessage = new DirectMessage
            {
                DirectChannelId = new Guid("0285A417-E6F4-402D-942E-E808F568EF43")
            };

            // Act
            Func<Task> act = async () => { await DirectChannelService.AddMessageAsync(directMessage); };

            //Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to add direct message. Channel { nameof(directMessage.DirectChannelId) }:{ directMessage.DirectChannelId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            _directChannelRepositoryMock.Setup(x => x.GetDirectChannelById(It.IsAny<Guid>()))
                .ReturnsAsync(new DirectChannel())
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var directMessage = new DirectMessage
            {
                DirectChannelId = new Guid("0285A417-E6F4-402D-942E-E808F568EF43"),
                OwnerId = new Guid("B7AF30DD-A06C-4621-A98F-2F4E9FB8076A")
            };

            // Act
            Func<Task> act = async () => { await DirectChannelService.AddMessageAsync(directMessage); };

            //Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get member. Member { nameof(directMessage.OwnerId) }:{ directMessage.OwnerId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldReturnDirectMessageResponse()
        {
            // Arrange
            _directChannelRepositoryMock.Setup(x => x.GetDirectChannelById(It.IsAny<Guid>()))
                .ReturnsAsync(new DirectChannel())
                .Verifiable();

            var memberId = new Guid("BE5C68F1-5983-4C08-B57B-FD4EFD7295B8");
            var member = new Member
            {
                Id = memberId
            };
            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(member)
                .Verifiable();

            var directMessage = new DirectMessage
            {
                Id = new Guid("53B9C55D-52F2-41E2-907D-4828B878FFA0"),
                DirectChannelId = new Guid("0285A417-E6F4-402D-942E-E808F568EF43"),
                OwnerId = new Guid("B7AF30DD-A06C-4621-A98F-2F4E9FB8076A")
            };

            var memberSummary = new MemberSummary();
            _domainModelsMapperMock.Setup(x => x.MapToMemberSummary(It.IsAny<Member>()))
                .Returns(memberSummary)
                .Verifiable();

            var directMessageResponse = new DirectMessageResponse();

            _directMessagesRepository.Setup(x => x.AddMessageAsync(It.IsAny<DirectMessage>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _domainModelsMapperMock.Setup(x => x.MapToDirectMessageResponse(It.Is<DirectMessage>(dm => dm.Equals(directMessage)),
                    It.Is<Member>(m => m.Equals(member))))
                .Returns(directMessageResponse)
                .Verifiable();

            // Act
            var act = await DirectChannelService.AddMessageAsync(directMessage);

            // Assert
            act.Should().BeEquivalentTo(directMessageResponse);
        }
    }
}
