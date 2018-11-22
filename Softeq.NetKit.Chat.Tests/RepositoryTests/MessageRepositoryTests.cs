// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Tests.Abstract;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.RepositoryTests
{
    public class MessageRepositoryTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("FE728AF3-DDE7-4B11-BD9B-55C3862262AA");
        private const string _memberName = "testMember";
        private readonly Guid _channelId = new Guid("FE728AF3-DDE7-4B11-BD9B-11C3862262EE");

        public MessageRepositoryTests()
        {
            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Active,
                Name = _memberName
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            var channel = new Channel
            {
                Id = _channelId,
                CreatorId = member.Id,
                Name = "testMessageChannel",
                Type = ChannelType.Public,
                MembersCount = 0
            };
            UnitOfWork.ChannelRepository.AddChannelAsync(channel).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task GetAllChannelMessagesAsyncTest()
        {
            // Arrange
            var messages = await UnitOfWork.MessageRepository.GetAllChannelMessagesAsync(_channelId);
            var expectedMessage = await GenerateAndAddMessage();

            // Act
            var messagesAfterAddingNew = await UnitOfWork.MessageRepository.GetAllChannelMessagesAsync(_channelId);

            // Assert
            Assert.NotNull(messages);
            Assert.Empty(messages);
            Assert.NotNull(messagesAfterAddingNew);
            Assert.NotEmpty(messagesAfterAddingNew);
            Assert.Single(messagesAfterAddingNew);
            AssertMessagesEqual(expectedMessage, messagesAfterAddingNew.First());
        }
        
        [Fact]
        public async Task GetOlderMessagesAsyncTest()
        {
            // Arrange
            var expectedMessage = await GenerateAndAddMessage();

            var createdDate = expectedMessage.Created;

            // Act 1
            var actualMessages = await UnitOfWork.MessageRepository.GetOlderMessagesAsync(
                _channelId, createdDate.Add(-TimeSpan.FromSeconds(1)), null);
            
            // Assert 1
            Assert.NotNull(actualMessages);
            Assert.Empty(actualMessages);

            // Act 2
            actualMessages = await UnitOfWork.MessageRepository.GetOlderMessagesAsync(
                _channelId, createdDate.Add(TimeSpan.FromSeconds(1)), null);

            // Assert 2
            Assert.NotNull(actualMessages);
            Assert.NotEmpty(actualMessages);
            Assert.Single(actualMessages);
            AssertMessagesEqual(expectedMessage, actualMessages.First());
        }

        [Fact]
        public async Task GetMessagesAsyncTest()
        {
            // Arrange
            var expectedMessage = await GenerateAndAddMessage();

            var createdDate = expectedMessage.Created;

            // Act 1
            var actualMessages = await UnitOfWork.MessageRepository.GetMessagesAsync(
                _channelId, createdDate.Add(TimeSpan.FromSeconds(1)), null);

            // Assert 1
            Assert.NotNull(actualMessages);
            Assert.Empty(actualMessages);

            // Act 2
            actualMessages = await UnitOfWork.MessageRepository.GetMessagesAsync(
                _channelId, createdDate.Add(-TimeSpan.FromSeconds(1)), null);

            // Assert 2
            Assert.NotNull(actualMessages);
            Assert.NotEmpty(actualMessages);
            Assert.Single(actualMessages);
            AssertMessagesEqual(expectedMessage, actualMessages.First());
        }

        [Fact]
        public async Task GetLastMessagesAsyncTest()
        {
            // Arrange
            var expectedMessage = await GenerateAndAddMessage();

            var createdDate = expectedMessage.Created;

            var pageSize = 0;
            var lastReadMessageCreated = createdDate.Add(TimeSpan.FromSeconds(1));

            // Act 1
            var actualMessages = await UnitOfWork.MessageRepository.GetLastMessagesAsync(
                _channelId, lastReadMessageCreated, pageSize);

            // Assert 1
            Assert.NotNull(actualMessages);
            Assert.Empty(actualMessages);

            // Arrange 2
            pageSize = 1;

            // Act 2
            actualMessages = await UnitOfWork.MessageRepository.GetLastMessagesAsync(
                _channelId, lastReadMessageCreated, pageSize);

            // Assert 2
            Assert.NotNull(actualMessages);
            Assert.NotEmpty(actualMessages);
            Assert.Single(actualMessages);
            AssertMessagesEqual(expectedMessage, actualMessages.First());

            // Arrange 3
            pageSize = 0;
            lastReadMessageCreated = createdDate.Add(-TimeSpan.FromSeconds(1));

            // Act 3
            actualMessages = await UnitOfWork.MessageRepository.GetLastMessagesAsync(
                _channelId, lastReadMessageCreated, pageSize);

            // Assert 3
            Assert.NotNull(actualMessages);
            Assert.NotEmpty(actualMessages);
            Assert.Single(actualMessages);
            AssertMessagesEqual(expectedMessage, actualMessages.First());
        }

        [Fact]
        public async Task GetPreviousMessageAsync_ShouldReturnPreviousMessageIfExists()
        {
            var firstMessage = await GenerateAndAddMessage();
            var previousForFirstMessage = await UnitOfWork.MessageRepository.GetPreviousMessageAsync(firstMessage);
            Assert.Null(previousForFirstMessage);

            var secondMessage = await GenerateAndAddMessage(DateTimeOffset.UtcNow.AddHours(1));
            var previousForSecondMessage = await UnitOfWork.MessageRepository.GetPreviousMessageAsync(secondMessage);
            previousForSecondMessage.Should().BeEquivalentTo(firstMessage, compareOptions => compareOptions.Excluding(message => message.Owner));
        }

        [Fact]
        public async Task GetPreviousMessagesAsyncTest()
        {
            // Arrange
            var firstMessage = await GenerateAndAddMessage();
            var secondMessage = await GenerateAndAddMessage(firstMessage.Created.AddMinutes(1));
            
            // Act 1
            var actualMessages = await UnitOfWork.MessageRepository.GetPreviousMessagesAsync(_channelId, firstMessage.Id);

            // Assert 1
            Assert.NotNull(actualMessages);
            Assert.Empty(actualMessages);

            // Act 2
            actualMessages = await UnitOfWork.MessageRepository.GetPreviousMessagesAsync(_channelId, secondMessage.Id);

            // Assert 2
            Assert.NotNull(actualMessages);
            Assert.NotEmpty(actualMessages);
            Assert.Single(actualMessages);
            AssertMessagesEqual(firstMessage, actualMessages.First());
        }

        [Fact]
        public async Task GetMessageByIdAsyncTest()
        {
            // Arrange
            var expectedMessage = await GenerateAndAddMessage();

            // Act
            var actualMessage = await UnitOfWork.MessageRepository.GetMessageByIdAsync(expectedMessage.Id);

            // Assert
            Assert.NotNull(actualMessage);
            AssertMessagesEqual(expectedMessage, actualMessage);
        }

        [Fact]
        public async Task GetLastReadMessageAsyncTest()
        {
            // Arrange
            var expectedMessage = await GenerateAndAddMessage();
            var anotherMessage = await GenerateAndAddMessage();

            // Act 1
            var actualMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(_memberId, _channelId);

            // Assert 1
            Assert.Null(actualMessage);

            // Arrange 2
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(new ChannelMembers
            {
                ChannelId = _channelId,
                MemberId = _memberId,
                LastReadMessageId = expectedMessage.Id
            });
            
            // Act 2
            actualMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(_memberId, _channelId);

            // Assert
            Assert.NotNull(actualMessage);
            AssertMessagesEqual(expectedMessage, actualMessage);
        }

        [Fact]
        public async Task AddMessageAsyncTest()
        {
            // Arrange
            var expectedMessage = await GenerateAndAddMessage();

            // Act
            var newMessage = await UnitOfWork.MessageRepository.GetMessageByIdAsync(expectedMessage.Id);

            // Assert
            Assert.NotNull(newMessage);
            AssertMessagesEqual(expectedMessage, newMessage);
        }

        [Fact]
        public async Task DeleteMessageAsyncTest()
        {
            // Arrange
            var expectedMessage = await GenerateAndAddMessage();

            // Act
            await UnitOfWork.MessageRepository.DeleteMessageAsync(expectedMessage.Id);
            var newMessage = await UnitOfWork.MessageRepository.GetMessageByIdAsync(expectedMessage.Id);

            // Assert
            Assert.Null(newMessage);
        }

        #region Private methods

        private async Task<Message> GenerateAndAddMessage(DateTimeOffset? customCreated = null)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                Body = "test",
                Created = customCreated ?? DateTimeOffset.UtcNow.AddMinutes(-10),
                ImageUrl = "test",
                Type = 0,
                ChannelId = _channelId,
                OwnerId = _memberId,
            };

            await UnitOfWork.MessageRepository.AddMessageAsync(message);

            return message;
        }

        private void AssertMessagesEqual(Message expectedMessage, Message actualMessage)
        {
            Assert.Equal(expectedMessage.Id, actualMessage.Id);
            Assert.Equal(expectedMessage.Body, actualMessage.Body);
            Assert.Equal(expectedMessage.Created, actualMessage.Created);
            Assert.Equal(expectedMessage.ImageUrl, actualMessage.ImageUrl);
            Assert.Equal(expectedMessage.Type, actualMessage.Type);
            Assert.Equal(expectedMessage.ChannelId, actualMessage.ChannelId);
            Assert.Equal(expectedMessage.OwnerId, actualMessage.OwnerId);
            if (expectedMessage.OwnerId == _memberId)
            {
                Assert.Equal(_memberName, actualMessage.Owner.Name);
            }
        }

        #endregion
    }
}
