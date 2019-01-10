// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberService;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberServiceTests
{
    public class GetMemberByIdAsyncTests : MemberServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var memberId = new Guid("BE5C68F1-5983-4C08-B57B-FD4EFD7295B8");

            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _memberService.GetMemberByIdAsync(memberId);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to get member by { nameof(memberId)}. Member { nameof(memberId)}:{ memberId} not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldReturnMemberSummary()
        {
            // Arrange
            var memberId = new Guid("BE5C68F1-5983-4C08-B57B-FD4EFD7295B8");

            Member member = new Member();
            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.Is<Guid>(id => id.Equals(memberId))))
                .ReturnsAsync(member)
                .Verifiable();

            MemberSummary memberSummary = new MemberSummary();
            _domainModelsMapperMock.Setup(x => x.MapToMemberSummary(It.Is<Member>(m => m.Equals(member))))
                .Returns(memberSummary)
                .Verifiable();

            // Act
            var act = await _memberService.GetMemberByIdAsync(memberId);

            // Assert
            act.Should().BeEquivalentTo(memberSummary);

            VerifyMocks();
        }
    }
}

