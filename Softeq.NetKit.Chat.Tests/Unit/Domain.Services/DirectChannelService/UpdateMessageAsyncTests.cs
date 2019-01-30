﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;
using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.DirectChannelService
{
    public class UpdateMessageAsyncTests : DirectChannelTestBase
    {
        [Fact]
        public void ShouldThrowIfMessageDoesNotExist()
        {
            // Arrange
            var messageId = new Guid("DA1D61EE-420F-4F8E-82AD-F2158358B0E0");

            _directMessagesRepository.Setup(x => x.GetMessageByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((DirectMessage)null)
                .Verifiable();

            var directMessage = new DirectMessage
            {
                Id = messageId,
                DirectChannelId = new Guid("0285A417-E6F4-402D-942E-E808F568EF43"),
                OwnerId = new Guid("B7AF30DD-A06C-4621-A98F-2F4E9FB8076A")
            };

            // Act
            Func<Task> act = async () => { await DirectChannelService.UpdateMessageAsync(directMessage); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get direct message. Message with {nameof(directMessage.Id)}:{directMessage.Id} is not found.");

            VerifyMocks();
        }

        [Fact]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            var messageId = new Guid("DA1D61EE-420F-4F8E-82AD-F2158358B0E0");

            var directMessage = new DirectMessage
            {
                Id = messageId,
                DirectChannelId = new Guid("0285A417-E6F4-402D-942E-E808F568EF43"),
                OwnerId = new Guid("B7AF30DD-A06C-4621-A98F-2F4E9FB8076A")
            };

            _directMessagesRepository.Setup(x => x.GetMessageByIdAsync(It.Is<Guid>(m => m.Equals(messageId))))
                .ReturnsAsync(directMessage)
                .Verifiable();

            _memberRepositoryMock.Setup(x => x.GetMemberByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            // Act
            Func<Task> act = async () => { await DirectChannelService.UpdateMessageAsync(directMessage); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get member. Member { nameof(directMessage.OwnerId) }:{ directMessage.OwnerId} is not found.");
        }

        [Fact]
        public async Task ShouldReturnDirectMessageResponse()
        {
        }
    }
}
