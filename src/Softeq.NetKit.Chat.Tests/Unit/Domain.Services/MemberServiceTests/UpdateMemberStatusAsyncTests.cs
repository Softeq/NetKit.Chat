// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberService;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberServiceTests
{
    public class UpdateMemberStatusAsyncTests : MemberServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var saasUserId = "8DCBF4A0-4490-45A2-B932-CEB71D19E9BD";
            UserStatus status = UserStatus.Online;

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _memberService.UpdateMemberStatusAsync(saasUserId, status);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to update member status. Member {nameof(saasUserId)}:{saasUserId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldUpdateMemberStatusAsync()
        {
            // Arrange
            var saasUserId = "8DCBF4A0-4490-45A2-B932-CEB71D19E9BD";
            UserStatus status = UserStatus.Online;

            var oldMember = new Member()
            {
                SaasUserId = saasUserId,
                Status = status
            };

            var utcNow = DateTimeOffset.UtcNow;
            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(utcNow)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saas => saas.Equals(oldMember.SaasUserId))))
                .ReturnsAsync(oldMember)
                .Verifiable();

            Member updatedMember = null;
            _memberRepositoryMock.Setup(x => x.UpdateMemberAsync(It.Is<Member>(m => m.Equals(oldMember))))
                .Callback<Member>(x => updatedMember = x)
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _memberService.UpdateMemberStatusAsync(saasUserId, status);

            // Assert
            updatedMember.Status.Should().BeEquivalentTo(oldMember.Status);
            updatedMember.LastActivity.Should().Subject.HasValue.Equals(utcNow);
        }
    }
}
