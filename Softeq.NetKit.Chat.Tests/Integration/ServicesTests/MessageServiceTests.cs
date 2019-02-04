// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.ServicesTests
{
    public class MessageServiceTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("8890278e-bad0-4394-a519-70585293de84");
        private readonly Guid _channelId = new Guid("7f4528a4-24d2-4f44-ac64-18fd56c19674");

        private const string SaasUserId = "f54cab3d-4fe0-4c25-9df1-019ec58ab564";

        private readonly IMessageService _messageService;
        private readonly IChannelService _channelService;

        public MessageServiceTests()
        {
            _channelService = LifetimeScope.Resolve<IChannelService>();
            _messageService = LifetimeScope.Resolve<IMessageService>();

            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online,
                SaasUserId = SaasUserId
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            var channel = new Channel
            {
                Id = _channelId,
                CreatorId = member.Id,
                Name = "testMessageService",
                MembersCount = 0,
                Type = ChannelType.Public
            };
            UnitOfWork.ChannelRepository.AddChannelAsync(channel).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task CreateMessageAsyncTest()
        {
            // Arrange
            var request = new CreateMessageRequest(SaasUserId, _channelId, MessageType.Default, "test");

            // Act
            var channel = await _channelService.GetChannelByIdAsync(_channelId);
            var oldChannelMessagesCount = await _channelService.GetChannelMessagesCountAsync(channel.Id);
            var message = await _messageService.CreateMessageAsync(request);
            var newChannel = await _channelService.GetChannelByIdAsync(_channelId);
            var newChannelMessagesCount = await _channelService.GetChannelMessagesCountAsync(newChannel.Id);
            
            // Assert
            Assert.NotNull(message);
            Assert.Equal(request.Body, message.Body);
            Assert.Equal(request.ChannelId, message.ChannelId);
            Assert.Equal(request.Type, message.Type);
            Assert.Equal(request.ImageUrl, message.ImageUrl);
            Assert.Equal(_channelId, message.ChannelId);
            Assert.True(newChannelMessagesCount > oldChannelMessagesCount);
        }

        [Fact]
        public async Task UpdateMessageAsync()
        {
            // Arrange
            var request = new CreateMessageRequest(SaasUserId, _channelId, MessageType.Default, "test");
            
            var message = await _messageService.CreateMessageAsync(request);

            var updatedRequest = new UpdateMessageRequest(SaasUserId, message.Id, "test2");

            // Act
            var updatedMessage = await _messageService.UpdateMessageAsync(updatedRequest);

            // Assert
            Assert.NotNull(updatedMessage);
            Assert.NotNull(updatedMessage.Updated);
            Assert.Equal(updatedRequest.Body, updatedMessage.Body);
        }

        [Fact]
        public async Task DeleteMessageAsyncTest()
        {
            // Arrange
            var request = new CreateMessageRequest(SaasUserId, _channelId, MessageType.Default, "test");

            var message = await _messageService.CreateMessageAsync(request);

            // Act
            var getMessagesRequest = new GetMessagesRequest(SaasUserId, _channelId, message.Id, message.Created, 10);
            var messages = await _messageService.GetMessagesAsync(getMessagesRequest);
            await _messageService.DisableMessageAsync(new DisabledMessageRequest(SaasUserId, message.Id));
            var newMessages = await _messageService.GetMessagesAsync(getMessagesRequest);

            // Assert
            Assert.NotNull(newMessages);
            Assert.Empty(newMessages.Results);
            Assert.True(messages.Results.Count() > newMessages.Results.Count());
        }

        [Fact]
        public async Task CreateMessageAsync_ShouldForwardMessageWithForwardType()
        {
            var defaultMessageRequest = new CreateMessageRequest(SaasUserId, _channelId, MessageType.Default, "this message supposed to be forwarded");
            var messageForForwarding = await _messageService.CreateMessageAsync(defaultMessageRequest);

            var forwardMessage = new CreateMessageRequest(SaasUserId, _channelId, MessageType.Forward, "comment for forwarded message")
            {
                ForwardedMessageId = messageForForwarding.Id
            };
            var forwardMessageResult = await _messageService.CreateMessageAsync(forwardMessage);

            forwardMessageResult.ForwardedMessage.Body.Should().BeEquivalentTo(messageForForwarding.Body);
        }
    }
}