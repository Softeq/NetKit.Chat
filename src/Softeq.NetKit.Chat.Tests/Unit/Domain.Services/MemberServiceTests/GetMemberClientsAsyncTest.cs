// Developed by Softeq Development Corporation
// http://www.softeq.co

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberServiceTests
{
    public class GetMemberClientsAsyncTest : MemberServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var memberId = new Guid("8DCBF4A0-4490-45A2-B932-CEB71D19E9BD");

            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _memberService.GetMemberClientsAsync(memberId);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to get member clients. Member {nameof(memberId)}:{memberId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldReturnClientsList()
        {
            // Arrange
            var memberId = new Guid("8DCBF4A0-4490-45A2-B932-CEB71D19E9BD");
            var member = new Member { Id = memberId };

            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.Is<Guid>(m => m.Equals(memberId))))
                .ReturnsAsync(member)
                .Verifiable();

            var clients = new List<Client>();

            _clientRepositoryMock.Setup(x => x.GetMemberClientsAsync(It.Is<Guid>(m => m.Equals(member.Id))))
                .ReturnsAsync(clients)
                .Verifiable();

            // Act
            var act = await _memberService.GetMemberClientsAsync(member.Id);

            // Assert
            act.Should().BeEquivalentTo(clients);

            VerifyMocks();
        }
    }
}
