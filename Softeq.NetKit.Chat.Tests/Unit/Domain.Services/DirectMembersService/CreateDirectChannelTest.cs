﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers;
using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.DirectMembersService
{
    public class CreateDirectChannelTest : DirectMessagesTestBase
    {
        [Fact]
        public void ShouldThrowIfOwnerDoesNotExist()
        {
            // Arrange
            var saasUserId = "4C21A8B9-75CD-43BA-9F18-2D86D479E9F0";
            var firstMemberId = new Guid("29B700AC-42E5-4148-8230-6A61E0648F32");
            var secondMemberId = new Guid("AC926107-F55C-49EC-A48E-B9985CEDB473");

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var createDirectMemberRequest = new CreateDirectChannelRequest(saasUserId, firstMemberId, secondMemberId);

            // Act
            Func<Task> act = async () => { await DirectMessageService.CreateDirectChannel(createDirectMemberRequest); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to create direct members. Member { nameof(createDirectMemberRequest.SaasUserId) }:{ createDirectMemberRequest.SaasUserId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var saasUserId = "4C21A8B9-75CD-43BA-9F18-2D86D479E9F0";
            var firstMemberId = new Guid("29B700AC-42E5-4148-8230-6A61E0648F32");
            var secondMemberId = new Guid("AC926107-F55C-49EC-A48E-B9985CEDB473");

            var firstMember = new Member { Id = firstMemberId };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saas => saas.Equals(saasUserId))))
                .ReturnsAsync(firstMember)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var createDirectMemberRequest = new CreateDirectChannelRequest(saasUserId, firstMemberId, secondMemberId);

            // Act
            Func<Task> act = async () => { await DirectMessageService.CreateDirectChannel(createDirectMemberRequest); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should()
                .Be($"Unable to create direct members. Member {nameof(createDirectMemberRequest.MemberId)}:{createDirectMemberRequest.MemberId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldReturnCreateDirectMemberResponse()
        {
            // Arrange
            var directMembersId = new Guid("CF5485B5-9613-447C-AE61-DCE038782F6D");
            var saasUserId = "4C21A8B9-75CD-43BA-9F18-2D86D479E9F0";
            var firstMemberId = new Guid("29B700AC-42E5-4148-8230-6A61E0648F32");
            var secondMemberId = new Guid("AC926107-F55C-49EC-A48E-B9985CEDB473");

            var firstMember = new Member { Id = firstMemberId };
            var secondMember = new Member { Id = secondMemberId };

            var createDirectMembersResponse = new DirectChannelResponse();

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saas => saas.Equals(saasUserId))))
                    .ReturnsAsync(firstMember)
                    .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.Is<Guid>(guid => guid.Equals(secondMemberId))))
                .ReturnsAsync(secondMember)
                .Verifiable();

            _directChannelRepositoryMock.Setup(x => x.CreateDirectChannel(It.Is<Guid>(id => id.Equals(directMembersId)),
                It.Is<Guid>(fm => fm.Equals(firstMemberId)), It.Is<Guid>(sm => sm.Equals(secondMemberId)))).Returns(Task.CompletedTask);

            var createDirectMemberRequest = new CreateDirectChannelRequest(saasUserId, firstMemberId, secondMemberId) { DirectChannelId = directMembersId };

            _domainModelsMapperMock.Setup(x => x.MapToDirectChannelResponse(
                    It.Is<Guid>(id => id.Equals(directMembersId)),
                    It.Is<Member>(fm => fm.Equals(firstMember)),
                    It.Is<Member>(sm => sm.Equals(secondMember))))
                .Returns(createDirectMembersResponse)
                .Verifiable();

            // Act
            var act = await DirectMessageService.CreateDirectChannel(createDirectMemberRequest);

            // Assert
            act.Should().BeEquivalentTo(createDirectMembersResponse);

            VerifyMocks();
        }
    }
}
