// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Linq;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Tests.Abstract;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.RepositoryTests
{
    public class MemberRepositoryTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("1a0781d2-f3d8-418a-810c-33e78b457678");
        private readonly Guid _memberId2 = new Guid("1e5f11cf-58b4-4b6a-b85f-de6188cf623f");
        private readonly Guid _channelId = new Guid("0af706fa-d820-4cde-9ccd-c189ed2561da");

        public MemberRepositoryTests()
        {
            var member = new Member
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
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            var channel = new Channel
            {
                Id = _channelId,
                CreatorId = member.Id,
                Name = "testMemberChannel",
                Type = ChannelType.Public,
                MembersCount = 0
            };
            UnitOfWork.ChannelRepository.AddChannelAsync(channel).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task AddMemberAsyncTest()
        {
            // Arrange
            var member = new Member
            {
                Id = _memberId2,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test2",
                SaasUserId = "test",
                Status = UserStatus.Active
            };

            // Act
            await UnitOfWork.MemberRepository.AddMemberAsync(member);
            var newMember = await UnitOfWork.MemberRepository.GetMemberByIdAsync(member.Id);

            // Assert
            Assert.NotNull(newMember);
            Assert.Equal(member.Id, newMember.Id);
            Assert.Equal(member.Email, newMember.Email);
            Assert.Equal(member.Role, newMember.Role);
            Assert.Equal(member.IsAfk, newMember.IsAfk);
            Assert.Equal(member.IsBanned, newMember.IsBanned);
            Assert.Equal(member.Name, newMember.Name);
            Assert.Equal(member.SaasUserId, newMember.SaasUserId);
            Assert.Equal(member.Status, newMember.Status);
        }

        [Fact]
        public async Task DeleteMemberAsyncTest()
        {
            // Arrange
            var member = new Member
            {
                Id = _memberId2,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test2",
                SaasUserId = "test",
                Status = UserStatus.Active
            };

            // Act
            await UnitOfWork.MemberRepository.DeleteMemberAsync(member.Id);
            var newMember = await UnitOfWork.MemberRepository.GetMemberByIdAsync(member.Id);

            // Assert
            Assert.Null(newMember);
        }

        [Fact]
        public async Task GetMemberByIdAsyncTest()
        {
            // Arrange
            var member = new Member
            {
                Id = _memberId2,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test2",
                SaasUserId = "test",
                Status = UserStatus.Active
            };

            // Act
            await UnitOfWork.MemberRepository.AddMemberAsync(member);
            var newMember = await UnitOfWork.MemberRepository.GetMemberByIdAsync(member.Id);

            // Assert
            Assert.NotNull(newMember);
            Assert.Equal(member.Id, newMember.Id);
            Assert.Equal(member.Email, newMember.Email);
            Assert.Equal(member.Role, newMember.Role);
            Assert.Equal(member.IsAfk, newMember.IsAfk);
            Assert.Equal(member.IsBanned, newMember.IsBanned);
            Assert.Equal(member.Name, newMember.Name);
            Assert.Equal(member.SaasUserId, newMember.SaasUserId);
            Assert.Equal(member.Status, newMember.Status);
        }

        [Fact]
        public async Task GetAllMembersAsyncTest()
        {
            // Arrange
            var member = new Member
            {
                Id = _memberId2,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test2",
                SaasUserId = "test",
                Status = UserStatus.Active
            };

            // Act
            var members = await UnitOfWork.MemberRepository.GetPagedMembersAsync(1, 10, string.Empty);
            await UnitOfWork.MemberRepository.AddMemberAsync(member);
            var newMembers = await UnitOfWork.MemberRepository.GetPagedMembersAsync(1, 10, string.Empty);

            // Assert
            Assert.NotNull(newMembers);
            Assert.NotEmpty(newMembers.Entities);
            Assert.True(newMembers.Entities.Count() > members.Entities.Count());
        }

        [Fact]
        public async Task GetAllOnlineMembersAsyncTest()
        {
            // Arrange
            var member = new Member
            {
                Id = _memberId2,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test2",
                SaasUserId = "test",
                Status = UserStatus.Active
            };

            // Act
            var members = await UnitOfWork.MemberRepository.GetAllOnlineMembersAsync();
            await UnitOfWork.MemberRepository.AddMemberAsync(member);
            var newMembers = await UnitOfWork.MemberRepository.GetAllOnlineMembersAsync();

            // Assert
            Assert.NotNull(members);
            Assert.Empty(members);
            Assert.NotNull(newMembers);
            Assert.NotEmpty(newMembers);
            Assert.True(newMembers.Count > members.Count);
            Assert.True(newMembers.Count == 1);
            Assert.Equal(member.Id, newMembers.First().Id);
            Assert.Equal(member.Email, newMembers.First().Email);
            Assert.Equal(member.Role, newMembers.First().Role);
            Assert.Equal(member.IsAfk, newMembers.First().IsAfk);
            Assert.Equal(member.IsBanned, newMembers.First().IsBanned);
            Assert.Equal(member.Name, newMembers.First().Name);
            Assert.Equal(member.SaasUserId, newMembers.First().SaasUserId);
            Assert.Equal(member.Status, newMembers.First().Status);
        }

        [Fact]
        public async Task GetMemberByClientIdAsyncTest()
        {
            // Arrange
            var client = new Client
            {
                Id = Guid.NewGuid(),
                MemberId = _memberId,
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow
            };

            // Act
            await UnitOfWork.ClientRepository.AddClientAsync(client);
            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(_memberId);
            var newMember = await UnitOfWork.MemberRepository.GetMemberByClientIdAsync(client.Id);

            // Assert
            Assert.NotNull(newMember);
            Assert.Equal(member.Id, newMember.Id);
            Assert.Equal(member.Email, newMember.Email);
            Assert.Equal(member.Role, newMember.Role);
            Assert.Equal(member.IsAfk, newMember.IsAfk);
            Assert.Equal(member.IsBanned, newMember.IsBanned);
            Assert.Equal(member.Name, newMember.Name);
            Assert.Equal(member.SaasUserId, newMember.SaasUserId);
            Assert.Equal(member.Status, newMember.Status);
        }

        [Fact]
        public async Task GetMemberByNameAsyncTest()
        {
            // Arrange
            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(_memberId);

            // Act
            var newMember = await UnitOfWork.MemberRepository.GetMemberByNameAsync(member.Name);

            // Assert
            Assert.NotNull(newMember);
            Assert.Equal(member.Id, newMember.Id);
            Assert.Equal(member.Email, newMember.Email);
            Assert.Equal(member.Role, newMember.Role);
            Assert.Equal(member.IsAfk, newMember.IsAfk);
            Assert.Equal(member.IsBanned, newMember.IsBanned);
            Assert.Equal(member.Name, newMember.Name);
            Assert.Equal(member.SaasUserId, newMember.SaasUserId);
            Assert.Equal(member.Status, newMember.Status);
        }

        [Fact]
        public async Task SearchMembersByNameAsyncTest()
        {
            // Arrange
            var member = new Member
            {
                Id = _memberId2,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test2",
                SaasUserId = "test",
                Status = UserStatus.Active
            };

            // Act
            await UnitOfWork.MemberRepository.AddMemberAsync(member);
            var members = await UnitOfWork.MemberRepository.SearchMembersByNameAsync("test");

            // Assert
            Assert.NotNull(members);
            Assert.NotEmpty(members);
            Assert.True(members.Count == 1);
        }

        [Fact]
        public async Task GetOnlineMembersInChannelAsyncTest()
        {
            // Arrange
            var member = new Member
            {
                Id = _memberId2,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test2",
                SaasUserId = "test",
                Status = UserStatus.Active
            };

            var channelMember = new ChannelMembers
            {
                MemberId = member.Id,
                ChannelId = _channelId,
                LastReadMessageId = null
            };

            // Act
            await UnitOfWork.MemberRepository.AddMemberAsync(member);
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
            var members = await UnitOfWork.MemberRepository.GetOnlineMembersInChannelAsync(_channelId);

            // Assert
            Assert.NotNull(members);
            Assert.NotEmpty(members);
            Assert.True(members.Count == 1);
            Assert.Equal(member.Id, members.First().Id);
            Assert.Equal(member.Email, members.First().Email);
            Assert.Equal(member.Role, members.First().Role);
            Assert.Equal(member.IsAfk, members.First().IsAfk);
            Assert.Equal(member.IsBanned, members.First().IsBanned);
            Assert.Equal(member.Name, members.First().Name);
            Assert.Equal(member.SaasUserId, members.First().SaasUserId);
            Assert.Equal(member.Status, members.First().Status);
        }
    }
}