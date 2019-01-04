// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MemberServiceTests
{
    public class GetClientsByMemberIdsTests : MemberServiceTestBase
    {
        [Fact]
        public async Task ShouldReturnClients()
        {
            // Arrange
            List<Guid> memberIds = new List<Guid>()
            {
                new Guid("C6E2A490-2022-41B6-824E-39C517D7D5C7"),
                new Guid("1DCA6B03-0AE5-48BA-A3BA-59F2AC3C0779"),
                new Guid("924EB81E-2F26-49BB-97CD-C65FEDC9EF05")
            };

            var clients = new List<Client>();
            _clientRepositoryMock.Setup(x => x.GetClientsWithMembersAsync(It.Is<List<Guid>>(ids => ids.Equals(memberIds))))
                .ReturnsAsync(clients)
                .Verifiable();

            var clientResponses = new List<ClientResponse>();
            foreach (var client in clients)
            {
                var clientResponse = new ClientResponse();

                _domainModelsMapperMock.Setup(x => x.MapToClientResponse(It.Is<Client>(c => c.Equals(client))))
                    .Returns(clientResponse)
                    .Verifiable();

                clientResponses.Add(clientResponse);
            }

            // Act
            var act = await _memberService.GetClientsByMemberIds(memberIds);

            // Assert
            act.Should().AllBeEquivalentTo(clientResponses);
        }
    }
}
