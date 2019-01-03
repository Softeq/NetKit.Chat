// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberService;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberServiceTests
{
    public class UpdateActivityAsyncTests : MemberServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var saasUserId = "5120CC44-C9CE-49B1-9ABA-6B7E5CF29E8B";
            var connectionId = "B47FCAE8-1817-4C78-8BBE-9D125B83E1BE";
            var userAgent = "testUserAgent";

            var updateMemberActivityRequest = new UpdateMemberActivityRequest(saasUserId, connectionId, userAgent);

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _memberService.UpdateActivityAsync(updateMemberActivityRequest);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to update activity. Member SaasUserId:{updateMemberActivityRequest.SaasUserId} not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfClientDoesNotExist()
        {
            // Arrange
            var saasUserId = "5120CC44-C9CE-49B1-9ABA-6B7E5CF29E8B";
            var connectionId = "B47FCAE8-1817-4C78-8BBE-9D125B83E1BE";
            var userAgent = "testUserAgent";

            var updateMemberActivityRequest = new UpdateMemberActivityRequest(saasUserId, connectionId, userAgent);

            var utcNow = DateTimeOffset.UtcNow;
            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(utcNow)
                .Verifiable();

            var member = new Member
            {
                Status = UserStatus.Active,
                IsAfk = false,
                LastActivity = utcNow
            };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(m => m.Equals(updateMemberActivityRequest.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.UpdateMemberAsync(It.Is<Member>(m => m.Equals(member))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _clientRepositoryMock.Setup(x => x.GetClientWithMemberAsync(It.IsAny<string>()))
                .ReturnsAsync((Client)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => await _memberService.UpdateActivityAsync(updateMemberActivityRequest);

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>().And.Message.Should()
                .Be($"Unable to update activity. Client {nameof(updateMemberActivityRequest.ConnectionId)}:{updateMemberActivityRequest.ConnectionId} not found.");

            VerifyMocks();
        }
    }
}
