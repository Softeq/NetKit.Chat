﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Client;
using Softeq.NetKit.Chat.Domain.Member;
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
            var client = new Connection
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
            var client = new Connection
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
            var client = new Connection
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
            var client = new Connection
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
    }
}