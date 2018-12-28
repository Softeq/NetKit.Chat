// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.ServicesTests
{
    public class MemberServiceTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("2c47a9d9-faf5-4ac2-92a4-d2770afc58e8");
        private readonly Guid _channelId = new Guid("d279799f-c205-4378-b734-0aef519d19f9");

        private const string SaasUserId = "4d048b6c-37b8-499a-a9e3-d3fe5211d5fc";

        private readonly IMemberService _memberService;
        private readonly IClientService _clientService;

        public MemberServiceTests()
        {
            _memberService = LifetimeScope.Resolve<IMemberService>();
            _clientService = LifetimeScope.Resolve<IClientService>();

            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTimeOffset.UtcNow,
                IsActive = false,
                Status = UserStatus.Online,
                SaasUserId = SaasUserId
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            var channel = new Channel
            {
                Id = _channelId,
                CreatorId = member.Id,
                Name = "testMemberService",
                MembersCount = 0,
                Type = ChannelType.Public
            };
            UnitOfWork.ChannelRepository.AddChannelAsync(channel).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task GetChannelMembersAsyncTest()
        {
            // Arrange
            await _memberService.InviteMemberAsync(_memberId, _channelId);

            // Act
            var members = await _memberService.GetChannelMembersAsync(_channelId);

            // Assert
            Assert.NotNull(members);
            Assert.NotEmpty(members);
            Assert.Equal(_memberId, members.First().Id);
            Assert.Equal(SaasUserId, members.First().SaasUserId);
            Assert.Equal(UserStatus.Online, members.First().Status);
        }

        [Fact]
        public async Task InviteMemberAsync_ShouldInviteMember()
        {
            // Act
            var members = await _memberService.GetChannelMembersAsync(_channelId);
            var channel = await _memberService.InviteMemberAsync(_memberId, _channelId);


            // Assert
            Assert.NotNull(channel);
            Assert.True(channel.MembersCount > 0);
            Assert.True(members.Count() < channel.MembersCount);
        }

        [Fact]
        public async Task ActivateMemberAsync_ShouldActivateMember()
        {
            var member = await _memberService.GetMemberByIdAsync(_memberId);

            Assert.False(member.IsActive);

            await _memberService.ActivateMemberAsync(member.SaasUserId);

            var activatedMember = await _memberService.GetMemberByIdAsync(_memberId);

            Assert.True(activatedMember.IsActive);
        }

        [Fact]
        public async Task AddClientAsync_ShouldCreateAndReturnNewClient()
        {
            var addClientRequest = new AddClientRequest(SaasUserId, "user@test.test", "7F97D474-3CBA-45E8-A90C-3955A3CBF59D", "user agent1", "user@test.test");

            var newClient = await _clientService.AddClientAsync(addClientRequest);

            Assert.NotNull(newClient);
            Assert.Equal(addClientRequest.SaasUserId, newClient.SaasUserId);
            Assert.Equal(addClientRequest.ConnectionId, newClient.ConnectionClientId);
            Assert.Equal(addClientRequest.UserName, newClient.UserName);
        }

        [Fact]
        public async Task UpdateMemberStatusAsync_ShouldUpdateMemberStatus()
        {
            var addClientRequest = new AddClientRequest(SaasUserId, "user@test.test", "7F97D474-3CBA-45E8-A90C-3955A3CBF59D", "user agent1", "user@test.test");
            var addedClient = await _clientService.AddClientAsync(addClientRequest);
            var createdMember = await _memberService.GetMemberBySaasUserIdAsync(addedClient.SaasUserId);

            Assert.Equal(UserStatus.Online, createdMember.Status);

            await _memberService.UpdateMemberStatusAsync(createdMember.SaasUserId, UserStatus.Offline);
            var changedMember = await _memberService.GetMemberBySaasUserIdAsync(addedClient.SaasUserId);

            Assert.Equal(UserStatus.Offline, changedMember.Status);
        }

        [Fact]
        public async Task UpdateMemberStatusAsync_ShouldSetMemberStatusOfflineIfNoClients()
        {
            var addClientRequest = new AddClientRequest(SaasUserId, "user@test.test", "7F97D474-3CBA-45E8-A90C-3955A3CBF59D", "user agent1", "user@test.test");
            var addedClient = await _clientService.AddClientAsync(addClientRequest);
            var member = await _memberService.GetMemberBySaasUserIdAsync(addedClient.SaasUserId);

            Assert.Equal(UserStatus.Online, member.Status);

            var clients = await _memberService.GetMemberClientsAsync(member.Id);
            foreach (var client in clients)
            {
                await _clientService.DeleteClientAsync(new DeleteClientRequest(client.ClientConnectionId));
            }

            var offlineMember = await _memberService.GetMemberBySaasUserIdAsync(addedClient.SaasUserId);
            Assert.Equal(UserStatus.Offline, offlineMember.Status);
        }
    }
}