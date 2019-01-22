// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.RepositoryTests
{
    public class DirectMembersRepositoryTests : BaseTest
    {
        private Guid _firstMemberId = new Guid("F5092772-3BEF-40C7-8FFD-2E0773DA3C1F");
        private Guid _secondMemberId = new Guid("8E5FCE8B-4BD3-4D97-AF1C-07FFD4933406");

        public DirectMembersRepositoryTests()
        {
            var firstMember = new Member
            {
                Id = _firstMemberId,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            var secondMember = new Member
            {
                Id = _secondMemberId,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            UnitOfWork.MemberRepository.AddMemberAsync(firstMember).GetAwaiter().GetResult();

            UnitOfWork.MemberRepository.AddMemberAsync(secondMember).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task CreateDirectMembers_ShouldReturnNewDirectMembers()
        {
            // Arrange
            var direct = new DirectMembers
            {
                Id = new Guid("456C30C4-F5CE-4EC1-AC56-EB9FD54EAB8E"),
                FirstMemberId = _firstMemberId,
                SecondMemberId = _secondMemberId
            };

            // Act
            await UnitOfWork.DirectMemberRepository.CreateDirectMembers(direct);

            var newDirectMembers = await UnitOfWork.DirectMemberRepository.GetDirectMembersById(direct.Id);

            // Assert
            newDirectMembers.Should().BeEquivalentTo(direct);
        }
    }
}
