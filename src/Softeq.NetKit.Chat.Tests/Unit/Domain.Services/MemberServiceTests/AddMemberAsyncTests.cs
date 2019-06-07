// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberServiceTests
{
    public class AddMemberAsyncTests : MemberServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberExists()
        {
            // Arrange
            var member = new Member
            {
                Id = new Guid("B83AA222-6EFA-4A4D-97DB-E3131C7AF81B"),
                SaasUserId = "8DCBF4A0-4490-45A2-B932-CEB71D19E9BD",
                Email = "test@test.com"
            };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new Member())
                .Verifiable();

            // Act
            Func<Task> act = async () => await _memberService.AddMemberAsync(member.SaasUserId, member.Email);

            // Assert
            act.Should().Throw<NetKitChatInvalidOperationException>().And.Message.Should()
                .Be($"Unable to add member. Member saasUserId:{member.SaasUserId} already exists.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldReturnMemberSummaryResponse()
        {
            // Arrange
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var utcNow = DateTimeOffset.UtcNow;
            _dateTimeProviderMock.Setup(x => x.GetUtcNow())
                .Returns(utcNow)
                .Verifiable();

            var email = "test@test.com";

            var newMember = new Member
            {
                Id = new Guid("B83AA222-6EFA-4A4D-97DB-E3131C7AF81B"),
                SaasUserId = "8DCBF4A0-4490-45A2-B932-CEB71D19E9BD",
                Email = email,
                Role = UserRole.User,
                IsBanned = false,
                Status = UserStatus.Online,
                LastActivity = utcNow,
                Name = email,
                LastNudged = utcNow,
                PhotoName = string.Empty,
                Messages = new List<Message>(),
                ConnectedClients = new List<Client>(),
                Channels = new List<ChannelMember>(),
                Notifications = new List<Notification>()
            };

            _memberRepositoryMock.Setup(x => x.AddMemberAsync(It.IsAny<Member>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var memberSummary = new MemberSummaryResponse
            {
                SaasUserId = newMember.SaasUserId,
                Email = newMember.Email
            };

            _domainModelsMapperMock.Setup(x => x.MapToMemberSummaryResponse(It.IsAny<Member>()))
                .Returns(memberSummary)
                .Verifiable();

            // Act
            var act = await _memberService.AddMemberAsync(newMember.SaasUserId, newMember.Email);

            // Assert
            VerifyMocks();

            act.Should().BeEquivalentTo(memberSummary);
        }
    }
}
