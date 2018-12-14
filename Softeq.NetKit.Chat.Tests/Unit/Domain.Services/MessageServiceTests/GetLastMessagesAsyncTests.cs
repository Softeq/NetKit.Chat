// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MessageServiceTests
{
    public class GetLastMessagesAsyncTests : MessageServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var request = new GetLastMessagesRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            // Act
            Func<Task> act = async () => { await _messageService.GetLastMessagesAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get last messages. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldReturnMessages_WhenFoundLastReadMessage()
        {
            // Arrange
            var request = new GetLastMessagesRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            var member = new Member { Id = new Guid("A1538EB3-4E4C-4E39-BDCB-F617003E4BBF") };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId => saasUserId.Equals(request.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            var lastReadMessage = new Message { Created = DateTimeOffset.UtcNow };
            _messageRepositoryMock.Setup(x => x.GetLastReadMessageAsync(
                    It.Is<Guid>(memberId => memberId.Equals(member.Id)),
                    It.Is<Guid>(channelId => channelId.Equals(request.ChannelId))))
                .ReturnsAsync(lastReadMessage)
                .Verifiable();

            var lastMessage = new Message { Id = new Guid("1E0DDFFA-3A21-44C8-97BB-E94B4F408680") };
            var lastMessages = new List<Message> { lastMessage };
            _messageRepositoryMock.Setup(x => x.GetLastMessagesWithOwnersAsync(
                    It.Is<Guid>(channelId => channelId.Equals(request.ChannelId)),
                    It.Is<DateTimeOffset>(lastReadMessageCreated => lastReadMessageCreated.Equals(lastReadMessage.Created)),
                    It.Is<int>(pageSize => pageSize.Equals(LastMessageReadCount))))
                .ReturnsAsync(lastMessages)
                .Verifiable();

            var messageResponse = new MessageResponse();
            _domainModelsMapperMock.Setup(x => x.MapToMessageResponse(
                    It.Is<Message>(message => message.Equals(lastMessage)),
                    It.Is<DateTimeOffset?>(lastReadMessageCreated => lastReadMessageCreated.Equals(lastReadMessage.Created))))
                .Returns(messageResponse)
                .Verifiable();

            // Act
            var result = await _messageService.GetLastMessagesAsync(request);

            // Assert
            VerifyMocks();
            result.Results.Should().BeEquivalentTo(new List<MessageResponse> { messageResponse });
        }

        [Fact]
        public async Task ShouldReturnMessages_WhenLastReadMessageNotFound()
        {
            // Arrange
            var request = new GetLastMessagesRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"));

            var member = new Member { Id = new Guid("A1538EB3-4E4C-4E39-BDCB-F617003E4BBF") };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId => saasUserId.Equals(request.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _messageRepositoryMock.Setup(x => x.GetLastReadMessageAsync(
                    It.Is<Guid>(memberId => memberId.Equals(member.Id)),
                    It.Is<Guid>(channelId => channelId.Equals(request.ChannelId))))
                .ReturnsAsync((Message)null)
                .Verifiable();

            var channelMessage = new Message { Id = new Guid("1E0DDFFA-3A21-44C8-97BB-E94B4F408680") };
            var allChannelMessages = new List<Message> { channelMessage };
            _messageRepositoryMock.Setup(x => x.GetAllChannelMessagesWithOwnersAsync(It.Is<Guid>(channelId => channelId.Equals(request.ChannelId))))
                .ReturnsAsync(allChannelMessages)
                .Verifiable();

            var messageResponse = new MessageResponse();
            _domainModelsMapperMock.Setup(x => x.MapToMessageResponse(
                    It.Is<Message>(message => message.Equals(channelMessage)),
                    It.Is<DateTimeOffset?>(lastReadMessageCreated => lastReadMessageCreated.Equals(null))))
                .Returns(messageResponse)
                .Verifiable();

            // Act
            var result = await _messageService.GetLastMessagesAsync(request);

            // Assert
            VerifyMocks();
            result.Results.Should().BeEquivalentTo(new List<MessageResponse> { messageResponse });
        }
    }
}