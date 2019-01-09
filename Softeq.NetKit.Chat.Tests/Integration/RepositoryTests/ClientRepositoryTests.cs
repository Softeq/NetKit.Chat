// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.RepositoryTests
{
    public class ClientRepositoryTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("FE728AF3-DDE7-4B11-BC9B-55C3862262AA");

        public ClientRepositoryTests()
        {
            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task GetMemberClientsAsync_ShouldReturnAllMemberClients()
        {
            var clients = new List<Client>
            {
                new Client
                {
                    Id = Guid.Parse("0258E264-A593-43AE-8344-000681FB7FDA"),
                    ClientConnectionId = Guid.NewGuid().ToString(),
                    LastActivity = DateTimeOffset.UtcNow,
                    LastClientActivity = DateTimeOffset.UtcNow,
                    Name = "Name 1",
                    MemberId = _memberId,
                    UserAgent = "test"
                },
                new Client
                {
                    Id = Guid.Parse("0258E264-A593-43AE-8344-000681FB7FDB"),
                    ClientConnectionId = Guid.NewGuid().ToString(),
                    LastActivity = DateTimeOffset.UtcNow,
                    LastClientActivity = DateTimeOffset.UtcNow,
                    Name = "Name 2",
                    MemberId = _memberId,
                    UserAgent = "test"
                }
            };
            foreach (var client in clients)
            {
                await UnitOfWork.ClientRepository.AddClientAsync(client);
            }

            var newClients = await UnitOfWork.ClientRepository.GetMemberClientsAsync(_memberId);

            newClients.Should().BeEquivalentTo(clients);
        }

        [Fact]
        public async Task GetNotMutedChannelClientConnectionIdsAsync_ShouldReturnNotMutedChannelClientConnectionIds()
        {
            // Arrange
            var firstNotMutedMember = new Member
            {
                Id = new Guid("15D2571D-9A64-4E3F-A0DB-4CBEF6315BD9"),
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online
            };
            await UnitOfWork.MemberRepository.AddMemberAsync(firstNotMutedMember);

            var secondNotMutedMember = new Member
            {
                Id = new Guid("E5516862-1704-4564-BF47-C1F2DBBE4E5B"),
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online
            };
            await UnitOfWork.MemberRepository.AddMemberAsync(secondNotMutedMember);

            var mutedMember = new Member
            {
                Id = new Guid("BD1510B1-A88D-4C81-8F08-92F17A1B3C2C"),
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online
            };
            await UnitOfWork.MemberRepository.AddMemberAsync(mutedMember);


            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                IsClosed = true,
                CreatorId = firstNotMutedMember.Id,
                Created = DateTimeOffset.UtcNow,
                Type = ChannelType.Public,
                MembersCount = 2
            };
            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);


            var firstNotMutedChannelMember = new ChannelMember
            {
                MemberId = firstNotMutedMember.Id,
                ChannelId = channel.Id,
                IsMuted = false
            };
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(firstNotMutedChannelMember);

            var secondNotMutedChannelMember = new ChannelMember
            {
                MemberId = secondNotMutedMember.Id,
                ChannelId = channel.Id,
                IsMuted = false
            };
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(secondNotMutedChannelMember);

            var mutedChannelMember = new ChannelMember
            {
                MemberId = mutedMember.Id,
                ChannelId = channel.Id,
                IsMuted = true
            };
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(mutedChannelMember);


            var firstNotMutedMemberClient1 = new Client
            {
                Id = Guid.Parse("89C0A79E-DAF2-4180-80A2-9FB0CBCBC1F9"),
                ClientConnectionId = "B7426717-F49E-4788-97AD-62DCCC123590",
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow,
                MemberId = firstNotMutedMember.Id
            };
            await UnitOfWork.ClientRepository.AddClientAsync(firstNotMutedMemberClient1);

            var firstNotMutedMemberClient2 = new Client
            {
                Id = Guid.Parse("B92A9753-D19F-489B-AC15-FD8EAF1AC85F"),
                ClientConnectionId = "D9C276A1-597A-40F3-9C21-6967CF42FF26",
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow,
                MemberId = firstNotMutedMember.Id
            };
            await UnitOfWork.ClientRepository.AddClientAsync(firstNotMutedMemberClient2);

            var secondNotMutedMemberClient = new Client
            {
                Id = Guid.Parse("D1F29BBA-AFF1-4780-8714-CE21237C4F76"),
                ClientConnectionId = "2708C531-4903-42D1-9B90-384F4B0E4AE6",
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow,
                MemberId = secondNotMutedMember.Id
            };
            await UnitOfWork.ClientRepository.AddClientAsync(secondNotMutedMemberClient);

            var mutedMemberClient = new Client
            {
                Id = Guid.Parse("396DBB59-545F-4948-AB89-5875C1EB2894"),
                ClientConnectionId = "461A3EC9-ABC0-4AA1-9652-A9B762785A5E",
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow,
                MemberId = mutedMember.Id
            };
            await UnitOfWork.ClientRepository.AddClientAsync(mutedMemberClient);


            // Act
            var clientConnectionIds = await UnitOfWork.ClientRepository.GetNotMutedChannelClientConnectionIdsAsync(channel.Id);


            // Assert
            var expectedClientConnectionIds = new List<string>
            {
                firstNotMutedMemberClient1.ClientConnectionId,
                firstNotMutedMemberClient2.ClientConnectionId,
                secondNotMutedMemberClient.ClientConnectionId
            };
            clientConnectionIds.Should().BeEquivalentTo(expectedClientConnectionIds);
        }

        [Fact]
        public async Task GetClientWithMemberAsync_ShouldGetClientWithMemberByClientConnectionId()
        {
            var member = new Member
            {
                Id = new Guid("68444A01-F562-4FE3-9714-5341655664B8"),
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online
            };
            await UnitOfWork.MemberRepository.AddMemberAsync(member);

            var client = new Client
            {
                Id = Guid.NewGuid(),
                ClientConnectionId = "80784772-35AA-416D-9D47-79621CBADE74",
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow,
                Name = "test",
                Member = member,
                MemberId = member.Id,
                UserAgent = "test"
            };
            await UnitOfWork.ClientRepository.AddClientAsync(client);

            var foundClient = await UnitOfWork.ClientRepository.GetClientWithMemberAsync(client.ClientConnectionId);

            foundClient.Should().BeEquivalentTo(client);
        }

        [Fact]
        public async Task AddClientAsync_ShouldAddClient()
        {
            var client = new Client
            {
                Id = Guid.NewGuid(),
                ClientConnectionId = Guid.NewGuid().ToString(),
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow,
                Name = "Name",
                MemberId = _memberId,
                UserAgent = "UserAgent"
            };

            await UnitOfWork.ClientRepository.AddClientAsync(client);

            var newClient = await UnitOfWork.ClientRepository.GetClientWithMemberAsync(client.ClientConnectionId);

            newClient.Should().BeEquivalentTo(client, options => options.Excluding(c => c.Member));
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldUpdateClient()
        {
            var originalClient = new Client
            {
                Id = new Guid("9AC1494F-DA85-41E7-9D4F-CEC9ED4E3CE8"),
                ClientConnectionId = "EAD90FAF-D0F7-4C8E-80AE-84A6E9CE08DD",
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow,
                Name = "client",
                MemberId = _memberId,
                UserAgent = "clientUserAgent"
            };
            await UnitOfWork.ClientRepository.AddClientAsync(originalClient);

            var changedClient = new Client
            {
                Id = originalClient.Id,
                ClientConnectionId = "5E8841DC-630C-44AC-B434-B43205ED2D00",
                LastActivity = originalClient.LastActivity.AddDays(1),
                LastClientActivity = originalClient.LastClientActivity.AddDays(1),
                Name = "changedClient",
                MemberId = originalClient.MemberId,
                UserAgent = "changedClientUserAgent"
            };

            await UnitOfWork.ClientRepository.UpdateClientAsync(changedClient);

            var updatedClient = await UnitOfWork.ClientRepository.GetClientWithMemberAsync(changedClient.ClientConnectionId);

            updatedClient.Should().BeEquivalentTo(changedClient, options => options.Excluding(client => client.Member));
        }

        [Fact]
        public async Task DeleteClientAsync_ShouldDeleteClient()
        {
            var client = new Client
            {
                Id = Guid.NewGuid(),
                ClientConnectionId = Guid.NewGuid().ToString(),
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow,
                Name = "test",
                MemberId = _memberId,
                UserAgent = "test"
            };
            await UnitOfWork.ClientRepository.AddClientAsync(client);

            await UnitOfWork.ClientRepository.DeleteClientAsync(client.Id);

            var removedClient = await UnitOfWork.ClientRepository.GetClientWithMemberAsync(client.ClientConnectionId);

            removedClient.Should().BeNull();
        }

        [Fact]
        public async Task GetClientsWithMembersAsync_ShouldReturnClientsWithMembers()
        {
            var member1 = new Member
            {
                Id = new Guid("15D2571D-9A64-4E3F-A0DB-4CBEF6315BD9"),
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online
            };
            await UnitOfWork.MemberRepository.AddMemberAsync(member1);

            var member2 = new Member
            {
                Id = new Guid("E5516862-1704-4564-BF47-C1F2DBBE4E5B"),
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online
            };
            await UnitOfWork.MemberRepository.AddMemberAsync(member2);


            var member1Client1 = new Client
            {
                Id = Guid.Parse("98484744-61CC-45F0-811C-BEE6C5CB3058"),
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow,
                Member = member1,
                MemberId = member1.Id
            };
            await UnitOfWork.ClientRepository.AddClientAsync(member1Client1);

            var member1Client2 = new Client
            {
                Id = Guid.Parse("48B7799C-8228-416E-89E5-CA7F0B473FB7"),
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow,
                Member = member1,
                MemberId = member1.Id
            };
            await UnitOfWork.ClientRepository.AddClientAsync(member1Client2);

            var member2Client = new Client
            {
                Id = Guid.Parse("4621684F-173E-42FB-BF16-64091C2B4BB4"),
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow,
                Member = member2,
                MemberId = member2.Id
            };
            await UnitOfWork.ClientRepository.AddClientAsync(member2Client);


            var memberIds = new List<Guid> { member1.Id, member2.Id };
            var clients = await UnitOfWork.ClientRepository.GetClientsWithMembersAsync(memberIds);


            var expectedClients = new List<Client>
            {
                member1Client1,
                member1Client2,
                member2Client
            };
            clients.Should().BeEquivalentTo(expectedClients);
        }

        [Fact]
        public async Task IsClientExistsAsync_ShouldReturnTrueIfClientExists()
        {
            var client = new Client
            {
                Id = Guid.NewGuid(),
                ClientConnectionId = Guid.NewGuid().ToString(),
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow,
                Name = "test",
                MemberId = _memberId,
                UserAgent = "test"
            };

            var exists = await UnitOfWork.ClientRepository.IsClientExistsAsync(client.ClientConnectionId);
            exists.Should().BeFalse();

            await UnitOfWork.ClientRepository.AddClientAsync(client);

            exists = await UnitOfWork.ClientRepository.IsClientExistsAsync(client.ClientConnectionId);
            exists.Should().BeTrue();
        }
    }
}