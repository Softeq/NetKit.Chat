// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Tests.Abstract;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.RepositoryTests
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
                Status = UserStatus.Active
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task AddClientAsyncTest()
        {
            // Arrange
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

            // Act
            await UnitOfWork.ClientRepository.AddClientAsync(client);
            var newClient = await UnitOfWork.ClientRepository.GetClientByIdAsync(client.Id);

            // Assert
            Assert.NotNull(newClient);
            Assert.Equal(client.Id, newClient.Id);
            Assert.Equal(client.ClientConnectionId, newClient.ClientConnectionId);
            Assert.Equal(client.Name, newClient.Name);
            Assert.Equal(client.MemberId, newClient.MemberId);
            Assert.Equal(client.UserAgent, newClient.UserAgent);
        }

        [Fact]
        public async Task DeleteClientAsyncTest()
        {
            // Arrange
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

            // Act
            await UnitOfWork.ClientRepository.AddClientAsync(client);
            await UnitOfWork.ClientRepository.DeleteClientAsync(client.Id);
            var newClient = await UnitOfWork.ClientRepository.GetClientByIdAsync(client.Id);

            // Assert
            Assert.Null(newClient);
        }

        [Fact]
        public async Task GetClientByIdAsyncTest()
        {
            // Arrange
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

            // Act
            await UnitOfWork.ClientRepository.AddClientAsync(client);
            var newClient = await UnitOfWork.ClientRepository.GetClientByIdAsync(client.Id);

            // Assert
            Assert.NotNull(newClient);
            Assert.Equal(client.Id, newClient.Id);
            Assert.Equal(client.ClientConnectionId, newClient.ClientConnectionId);
            Assert.Equal(client.Name, newClient.Name);
            Assert.Equal(client.MemberId, newClient.MemberId);
            Assert.Equal(client.UserAgent, newClient.UserAgent);
        }

        [Fact]
        public async Task GetClientsAsyncTest()
        {
            // Arrange
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

            // Act
            var clients = await UnitOfWork.ClientRepository.GetAllClientsAsync();
            await UnitOfWork.ClientRepository.AddClientAsync(client);
            var newClients = await UnitOfWork.ClientRepository.GetAllClientsAsync();

            // Assert
            Assert.NotNull(clients);
            Assert.Empty(clients);
            Assert.NotNull(newClients);
            Assert.NotEmpty(newClients);
            Assert.True(newClients.Count > clients.Count);
        }

        [Fact]
        public async Task GetMemberClientsAsync_ShouldReturnAllMemberClients()
        {
            var client = new Client
            {
                Id = Guid.Parse("0258E264-A593-43AE-8344-000681FB7FDA"),
                ClientConnectionId = Guid.NewGuid().ToString(),
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow,
                Name = "test 1",
                MemberId = _memberId,
                UserAgent = "test"
            };
            var client2 = new Client
            {
                Id = Guid.Parse("0258E264-A593-43AE-8344-000681FB7FDB"),
                ClientConnectionId = Guid.NewGuid().ToString(),
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = DateTimeOffset.UtcNow,
                Name = "test 2",
                MemberId = _memberId,
                UserAgent = "test"
            };
           
            await UnitOfWork.ClientRepository.AddClientAsync(client);
            await UnitOfWork.ClientRepository.AddClientAsync(client2);
            var newClients = await UnitOfWork.ClientRepository.GetMemberClientsAsync(_memberId);
           
            Assert.Equal(2, newClients.Count);
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldUpdateClient()
        {
            var originalClient = new Client
            {
                Id = new Guid("9AC1494F-DA85-41E7-9D4F-CEC9ED4E3CE8"),
                ClientConnectionId = new Guid("EAD90FAF-D0F7-4C8E-80AE-84A6E9CE08DD").ToString(),
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
                ClientConnectionId = new Guid("5E8841DC-630C-44AC-B434-B43205ED2D00").ToString(),
                LastActivity = originalClient.LastActivity.AddDays(1),
                LastClientActivity = originalClient.LastClientActivity.AddDays(1),
                Name = "changedClient",
                MemberId = originalClient.MemberId,
                UserAgent = "changedClientUserAgent"
            };
            await UnitOfWork.ClientRepository.UpdateClientAsync(changedClient);

            var updatedClient = await UnitOfWork.ClientRepository.GetClientByIdAsync(originalClient.Id);

            updatedClient.Should().BeEquivalentTo(changedClient, compareOptions => compareOptions.Excluding(client => client.Member));
        }
    }
}