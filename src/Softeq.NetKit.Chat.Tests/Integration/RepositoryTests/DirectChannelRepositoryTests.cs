// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.RepositoryTests
{
    public class DirectChannelRepositoryTests : BaseTest
    {
        private Guid _ownerId = new Guid("F5092772-3BEF-40C7-8FFD-2E0773DA3C1F");
        private Guid _memberId = new Guid("8E5FCE8B-4BD3-4D97-AF1C-07FFD4933406");

        public DirectChannelRepositoryTests()
        {
            var firstMember = new Member
            {
                Id = _ownerId,
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
                Id = _memberId,
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
        [Trait("Category", "Integration")]
        public async Task CreateDirectChannel_ShouldReturnNewDirectChannel()
        {
            // Arrange
            var id = new Guid("B815B750-BB22-45E0-B332-0EE39D9A7A5C");

            // Act
            await UnitOfWork.DirectChannelRepository.CreateDirectChannelAsync(id, _ownerId, _memberId);
            var directChannel = await UnitOfWork.DirectChannelRepository.GetDirectChannelAsync(id);

            // Assert
            directChannel.Id.Should().Be(id);
            directChannel.OwnerId.Should().Be(_ownerId);
            directChannel.MemberId.Should().Be(_memberId);
        }
    }
}
