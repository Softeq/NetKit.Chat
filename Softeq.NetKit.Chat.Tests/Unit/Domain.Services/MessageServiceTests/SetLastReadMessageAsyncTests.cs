// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Domain.Services.MessageServiceTests
{
    public class SetLastReadMessageAsyncTests : MessageServiceTestBase
    {
        [Fact]
        public void ShouldThrowIfMemberDoesNotExist()
        {
            // Arrange
            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync((Member)null)
                .Verifiable();

            var request = new SetLastReadMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), new Guid("01ADF1C2-0B38-4E4F-9E36-0C0BC7908062"));

            // Act
            Func<Task> act = async () => { await _messageService.SetLastReadMessageAsync(request); };

            // Assert
            act.Should().Throw<NetKitChatNotFoundException>()
                .And.Message.Should().Be($"Unable to set last read message. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");

            VerifyMocks();
        }

        [Fact]
        public async Task ShouldSetLastReadMessage()
        {
            // Arrange
            var request = new SetLastReadMessageRequest("864EB62D-D833-47FA-8A88-DDBFE76AE6A7", new Guid("A455F139-09E6-4EF5-B55A-D4C94D05DFDE"), new Guid("01ADF1C2-0B38-4E4F-9E36-0C0BC7908062"));

            var member = new Member { Id = new Guid("A1538EB3-4E4C-4E39-BDCB-F617003E4BBF") };

            _memberRepositoryMock.Setup(x => x.GetMemberBySaasUserIdAsync(It.Is<string>(saasUserId => saasUserId.Equals(request.SaasUserId))))
                .ReturnsAsync(member)
                .Verifiable();
            
            _channelMemberRepositoryMock.Setup(x => x.SetLastReadMessageAsync(
                    It.Is<Guid>(memberId => memberId.Equals(member.Id)),
                    It.Is<Guid>(channelId => channelId.Equals(request.ChannelId)),
                    It.Is<Guid>(messageId => messageId.Equals(request.MessageId))))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _messageService.SetLastReadMessageAsync(request);
            
            // Assert
            VerifyMocks();
        }
    }
}