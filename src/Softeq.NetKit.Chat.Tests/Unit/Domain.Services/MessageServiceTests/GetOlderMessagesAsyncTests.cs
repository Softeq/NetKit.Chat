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
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Message;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MessageServiceTests
{
    public class GetOlderMessagesAsyncTests : MessageServiceTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var request = new GetMessagesRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), new Guid("01ADF1C2-0B38-4E4F-9E36-0C0BC7908062"), DateTimeOffset.Now, null);

            // Act
            Func<Task> act = async () => { await _messageService.GetOlderMessagesAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to get older messages. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");

            VerifyMocks();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldReturnOlderMessages_WhenFoundLastMessageByMessageId_AndFoundLastReadMessage()
        {
            // Arrange
            var request = new GetMessagesRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), new Guid("01ADF1C2-0B38-4E4F-9E36-0C0BC7908062"), DateTimeOffset.Now, 10);

            var member = new Member { Id = new Guid("A1538EB3-4E4C-4E39-BDCB-F617003E4BBF") };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId => saasUserId.Equals(request.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            var lastMessage = new Message { Created = DateTimeOffset.Now };
            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .ReturnsAsync(lastMessage)
                .Verifiable();

            var lastReadMessage = new Message { Created = DateTimeOffset.UtcNow.AddMinutes(1) };
            _messageRepositoryMock.Setup(x => x.GetLastReadMessageAsync(
                    It.Is<Guid>(memberId => memberId.Equals(member.Id)),
                    It.Is<Guid>(channelId => channelId.Equals(request.ChannelId))))
                .ReturnsAsync(lastReadMessage)
                .Verifiable();

            var olderMessage = new Message { Id = new Guid("1E0DDFFA-3A21-44C8-97BB-E94B4F408680") };
            var olderMessages = new List<Message> { olderMessage };
            _messageRepositoryMock.Setup(x => x.GetOlderMessagesWithOwnersAsync(
                    It.Is<Guid>(channelId => channelId.Equals(request.ChannelId)),
                    It.Is<DateTimeOffset>(lastReadMessageCreated => lastReadMessageCreated.Equals(lastMessage.Created)),
                    It.Is<int?>(pageSize => pageSize.Equals(request.PageSize))))
                .ReturnsAsync(olderMessages)
                .Verifiable();

            var messageResponse = new MessageResponse();
            _domainModelsMapperMock.Setup(x => x.MapToMessageResponse(
                    It.Is<Message>(message => message.Equals(olderMessage)),
                    It.Is<DateTimeOffset?>(lastReadMessageCreated => lastReadMessageCreated.Equals(lastReadMessage.Created))))
                .Returns(messageResponse)
                .Verifiable();

            // Act
            var result = await _messageService.GetOlderMessagesAsync(request);

            // Assert
            VerifyMocks();
            result.PageSize.Should().Be(request.PageSize);
            result.Results.Should().BeEquivalentTo(new List<MessageResponse> { messageResponse });
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ShouldReturnOlderMessages_WhenLastMessageByMessageIdNotFound_AndLastReadMessageNotFound()
        {
            // Arrange
            var request = new GetMessagesRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), new Guid("01ADF1C2-0B38-4E4F-9E36-0C0BC7908062"), DateTimeOffset.Now, 10);

            var member = new Member { Id = new Guid("A1538EB3-4E4C-4E39-BDCB-F617003E4BBF") };
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId => saasUserId.Equals(request.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();

            _messageRepositoryMock.Setup(x => x.GetMessageWithOwnerAndForwardMessageAsync(It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .ReturnsAsync((Message)null)
                .Verifiable();

            _messageRepositoryMock.Setup(x => x.GetLastReadMessageAsync(
                    It.Is<Guid>(memberId => memberId.Equals(member.Id)),
                    It.Is<Guid>(channelId => channelId.Equals(request.ChannelId))))
                .ReturnsAsync((Message)null)
                .Verifiable();

            var olderMessage = new Message { Id = new Guid("1E0DDFFA-3A21-44C8-97BB-E94B4F408680") };
            var olderMessages = new List<Message> { olderMessage };
            _messageRepositoryMock.Setup(x => x.GetOlderMessagesWithOwnersAsync(
                    It.Is<Guid>(channelId => channelId.Equals(request.ChannelId)),
                    It.Is<DateTimeOffset>(lastReadMessageCreated => lastReadMessageCreated.Equals(request.MessageCreatedDate)),
                    It.Is<int?>(pageSize => pageSize.Equals(request.PageSize))))
                .ReturnsAsync(olderMessages)
                .Verifiable();

            var messageResponse = new MessageResponse();
            _domainModelsMapperMock.Setup(x => x.MapToMessageResponse(
                    It.Is<Message>(message => message.Equals(olderMessage)),
                    It.Is<DateTimeOffset?>(lastReadMessageCreated => lastReadMessageCreated.Equals(null))))
                .Returns(messageResponse)
                .Verifiable();

            // Act
            var result = await _messageService.GetOlderMessagesAsync(request);

            // Assert
            VerifyMocks();
            result.PageSize.Should().Be(request.PageSize);
            result.Results.Should().BeEquivalentTo(new List<MessageResponse> { messageResponse });
        }
    }
}