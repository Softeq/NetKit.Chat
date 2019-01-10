// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberService
{
    public class GetMemberBySaasUserIdAsyncTests : MemberServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var saasUserId = "7657C4D0-D5D3-4F8A-97E0-70AC1CEAF895";

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _memberService.GetMemberBySaasUserIdAsync(saasUserId);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to get member by {nameof(saasUserId)}. Member {nameof(saasUserId)}:{saasUserId} not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldReturnMemberSummary()
        {
            // Arrange
            var saasUserId = "9190FEB0-FDF0-4DDD-AC92-750D8AA33DC2";

            Member member = new Member();
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saas => saas.Equals(saasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            MemberSummary memberSummary = new MemberSummary();
            _domainModelsMapperMock.Setup(x => x.MapToMemberSummary(It.Is<Member>(m => m.Equals(member))))
                .Returns(memberSummary)
                .Verifiable();

            // Act
            var act = await _memberService.GetMemberBySaasUserIdAsync(saasUserId);

            // Assert
            act.Should().BeEquivalentTo(memberSummary);

            VerifyMocks();
        }
    }
}
