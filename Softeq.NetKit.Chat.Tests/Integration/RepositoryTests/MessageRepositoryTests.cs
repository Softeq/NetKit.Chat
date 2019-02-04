// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.RepositoryTests
{
    public class MessageRepositoryTests : BaseTest
    {
        private readonly Member _member;
        private readonly Guid _channelId = new Guid("FE728AF3-DDE7-4B11-BD9B-11C3862262EE");

        public MessageRepositoryTests()
        {
            _member = new Member
            {
                Id = new Guid("FE728AF3-DDE7-4B11-BD9B-55C3862262AA"),
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online,
                Name = "testMember"
            };
            UnitOfWork.MemberRepository.AddMemberAsync(_member).GetAwaiter().GetResult();

            var channel = new Channel
            {
                Id = _channelId,
                CreatorId = _member.Id
            };
            UnitOfWork.ChannelRepository.AddChannelAsync(channel).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task GetAllChannelMessagesWithOwnersAsync_ShouldReturnAllChannelMessages()
        {
            // Arrange
            var expectedChannelMessages = new List<Message>();
            for (var i = 0; i < 5; i++)
            {
                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    Body = "Body",
                    Created = DateTimeOffset.Now,
                    ImageUrl = "ImageUrl",
                    Type = MessageType.Default,
                    ChannelId = _channelId,
                    OwnerId = _member.Id,
                    Owner = _member,
                    Updated = DateTimeOffset.Now,
                    AccessibilityStatus = AccessibilityStatus.Present
                };
                await UnitOfWork.MessageRepository.AddMessageAsync(message);
                expectedChannelMessages.Add(message);
            }

            // Add second channel with message
            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                CreatorId = _member.Id
            };
            await UnitOfWork.ChannelRepository.AddChannelAsync(channel);

            var messageInSecondChannel = new Message
            {
                Id = Guid.NewGuid(),
                Body = "Body",
                Created = DateTimeOffset.Now,
                ImageUrl = "ImageUrl",
                Type = MessageType.Default,
                ChannelId = channel.Id,
                OwnerId = _member.Id,
                Updated = DateTimeOffset.Now,
                AccessibilityStatus = AccessibilityStatus.Present
            };
            await UnitOfWork.MessageRepository.AddMessageAsync(messageInSecondChannel);

            // Act
            var channelMessages = await UnitOfWork.MessageRepository.GetAllChannelMessagesWithOwnersAsync(_channelId);

            // Assert
            channelMessages.Should().BeEquivalentTo(expectedChannelMessages);
        }

        [Fact]
        public async Task GetOlderMessagesWithOwnersAsync_ShouldReturnOlderMessagesWithOwner_which_Created_field_less_than_LastReadMessageCreated()
        {
            // Arrange
            var lastReadMessage = new Message
            {
                Id = Guid.NewGuid(),
                Body = "Body",
                Created = DateTimeOffset.Now,
                ImageUrl = "ImageUrl",
                Type = MessageType.Default,
                ChannelId = _channelId,
                OwnerId = _member.Id,
                Owner = _member,
                Updated = DateTimeOffset.Now,
                AccessibilityStatus = AccessibilityStatus.Present
            };
            await UnitOfWork.MessageRepository.AddMessageAsync(lastReadMessage);

            // Add unread messages
            for (var i = 1; i < 5; i++)
            {
                var unreadMessage = new Message
                {
                    Id = Guid.NewGuid(),
                    Body = "Body",
                    Created = lastReadMessage.Created + TimeSpan.FromSeconds(i),
                    ImageUrl = "ImageUrl",
                    Type = MessageType.Default,
                    ChannelId = _channelId,
                    OwnerId = _member.Id,
                    Owner = _member,
                    Updated = DateTimeOffset.Now,
                    AccessibilityStatus = AccessibilityStatus.Present
                };
                await UnitOfWork.MessageRepository.AddMessageAsync(unreadMessage);
            }

            // Add already read messages
            var readMessages = new List<Message>();
            for (var i = 1; i < 5; i++)
            {
                var readMessage = new Message
                {
                    Id = Guid.NewGuid(),
                    Body = $"Body{i}",
                    Created = lastReadMessage.Created - TimeSpan.FromSeconds(i),
                    ImageUrl = $"ImageUrl{i}",
                    Type = MessageType.Default,
                    ChannelId = _channelId,
                    OwnerId = _member.Id,
                    Owner = _member,
                    Updated = DateTimeOffset.Now - TimeSpan.FromSeconds(i),
                    AccessibilityStatus = AccessibilityStatus.Present
                };
                await UnitOfWork.MessageRepository.AddMessageAsync(readMessage);
                readMessages.Add(readMessage);
            }
            readMessages = readMessages.OrderBy(o => o.Created).ToList();


            // Act
            var allOlderMessages = await UnitOfWork.MessageRepository.GetOlderMessagesWithOwnersAsync(_channelId, lastReadMessage.Created, null);
            var firstTwoOlderMessages = await UnitOfWork.MessageRepository.GetOlderMessagesWithOwnersAsync(_channelId, lastReadMessage.Created, 2);

            // Assert
            allOlderMessages.Should().BeEquivalentTo(readMessages);
            firstTwoOlderMessages.Should().BeEquivalentTo(readMessages.Skip(2));
        }

        [Fact]
        public async Task GetMessagesWithOwnersAsync_ShouldReturnMessages()
        {
            // Arrange
            var lastReadMessage = new Message
            {
                Id = Guid.NewGuid(),
                Body = "Body",
                Created = DateTimeOffset.Now,
                ImageUrl = "ImageUrl",
                Type = MessageType.Default,
                ChannelId = _channelId,
                Owner = _member,
                OwnerId = _member.Id,
                Updated = DateTimeOffset.Now,
                AccessibilityStatus = AccessibilityStatus.Present
            };
            await UnitOfWork.MessageRepository.AddMessageAsync(lastReadMessage);

            // Add unread messages
            var unreadMessages = new List<Message>
            {
                lastReadMessage
            };

            for (var i = 1; i < 5; i++)
            {
                var unreadMessage = new Message
                {
                    Id = Guid.NewGuid(),
                    Body = $"Body{i}",
                    Created = lastReadMessage.Created + TimeSpan.FromSeconds(i),
                    ImageUrl = $"ImageUrl{i}",
                    Type = MessageType.Default,
                    ChannelId = _channelId,
                    Owner = _member,
                    OwnerId = _member.Id,
                    Updated = DateTimeOffset.Now,
                    AccessibilityStatus = AccessibilityStatus.Present
                };
                await UnitOfWork.MessageRepository.AddMessageAsync(unreadMessage);
                unreadMessages.Add(unreadMessage);
            }
            unreadMessages = unreadMessages.OrderBy(o => o.Created).ToList();


            // Add already read messages
            for (var i = 1; i < 5; i++)
            {
                var readMessage = new Message
                {
                    Id = Guid.NewGuid(),
                    Body = $"Body{i}",
                    Created = lastReadMessage.Created - TimeSpan.FromSeconds(i),
                    ImageUrl = $"ImageUrl{i}",
                    Type = MessageType.Default,
                    ChannelId = _channelId,
                    OwnerId = _member.Id,
                    Owner = _member,
                    Updated = DateTimeOffset.Now - TimeSpan.FromSeconds(i),
                    AccessibilityStatus = AccessibilityStatus.Present
                };
                await UnitOfWork.MessageRepository.AddMessageAsync(readMessage);
            }


            // Act
            var allNewMessages = await UnitOfWork.MessageRepository.GetMessagesWithOwnersAsync(_channelId, lastReadMessage.Created, null);
            var firstTwoOlderMessages = await UnitOfWork.MessageRepository.GetMessagesWithOwnersAsync(_channelId, lastReadMessage.Created, 2);

            // Assert
            allNewMessages.Should().BeEquivalentTo(unreadMessages);
            firstTwoOlderMessages.Should().BeEquivalentTo(unreadMessages.Take(2));
        }

        [Fact]
        public async Task GetLastMessagesWithOwnersAsync_ShouldReturnLastMessagesWithOwner()
        {
            // Arrange
            var lastReadMessage = new Message
            {
                Id = Guid.NewGuid(),
                Body = "Body",
                Created = DateTimeOffset.Now,
                ImageUrl = "ImageUrl",
                Type = MessageType.Default,
                ChannelId = _channelId,
                Owner = _member,
                OwnerId = _member.Id,
                Updated = DateTimeOffset.Now,
                AccessibilityStatus = AccessibilityStatus.Present
            };
            await UnitOfWork.MessageRepository.AddMessageAsync(lastReadMessage);

            // Add unread messages
            var unreadMessages = new List<Message>();

            for (var i = 1; i < 5; i++)
            {
                var unreadMessage = new Message
                {
                    Id = Guid.NewGuid(),
                    Body = $"Body{i}",
                    Created = lastReadMessage.Created + TimeSpan.FromSeconds(i),
                    ImageUrl = $"ImageUrl{i}",
                    Type = MessageType.Default,
                    ChannelId = _channelId,
                    Owner = _member,
                    OwnerId = _member.Id,
                    Updated = DateTimeOffset.Now,
                    AccessibilityStatus = AccessibilityStatus.Present
                };
                await UnitOfWork.MessageRepository.AddMessageAsync(unreadMessage);
                unreadMessages.Add(unreadMessage);
            }
            unreadMessages = unreadMessages.OrderBy(o => o.Created).ToList();


            // Add already read messages
            var readMessages = new List<Message>
            {
                lastReadMessage
            };
            for (var i = 1; i < 5; i++)
            {
                var readMessage = new Message
                {
                    Id = Guid.NewGuid(),
                    Body = $"Body{i}",
                    Created = lastReadMessage.Created - TimeSpan.FromSeconds(i),
                    ImageUrl = $"ImageUrl{i}",
                    Type = MessageType.Default,
                    ChannelId = _channelId,
                    OwnerId = _member.Id,
                    Owner = _member,
                    Updated = DateTimeOffset.Now - TimeSpan.FromSeconds(i),
                    AccessibilityStatus = AccessibilityStatus.Present
                };
                await UnitOfWork.MessageRepository.AddMessageAsync(readMessage);
                readMessages.Add(readMessage);
            }

            // Act
            var messages = await UnitOfWork.MessageRepository.GetLastMessagesWithOwnersAsync(_channelId, lastReadMessage.Created, 2);

            // Assert
            var someReadAndAllUnreadMessages = new List<Message>();
            someReadAndAllUnreadMessages.AddRange(readMessages.Take(2));
            someReadAndAllUnreadMessages.AddRange(unreadMessages);
            messages.Should().BeEquivalentTo(someReadAndAllUnreadMessages);
        }

        [Fact]
        public async Task GetMessageWithOwnerAndForwardedMessageAsync_ShouldReturnMessageWithOwnerAndForwardMessage()
        {
            var messageId = new Guid("D459FCFB-F436-4B80-AD08-670630109C9C");
            var expectedMessage = new Message
            {
                Id = messageId,
                Body = "Body",
                Created = DateTimeOffset.UtcNow,
                ImageUrl = "ImageUrl",
                Type = MessageType.Default,
                ChannelId = _channelId,
                Owner = _member,
                OwnerId = _member.Id,
                Updated = DateTimeOffset.Now,
                ForwardMessageId = messageId,
                AccessibilityStatus = AccessibilityStatus.Present
            };
            await UnitOfWork.MessageRepository.AddMessageAsync(expectedMessage);

            var forwardMessage = new ForwardMessage
            {
                Id = expectedMessage.Id,
                Body = "Body",
                ChannelId = _channelId,
                OwnerId = _member.Id,
                Created = DateTimeOffset.UtcNow
            };
            await UnitOfWork.ForwardMessageRepository.AddForwardMessageAsync(forwardMessage);

            expectedMessage.ForwardMessageId = forwardMessage.Id;
            expectedMessage.ForwardedMessage = forwardMessage;


            var actualMessage = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(expectedMessage.Id);


            actualMessage.Should().BeEquivalentTo(expectedMessage);
        }

        [Fact]
        public async Task GetPreviousMessageAsync_ShouldReturnPrevMessageIfExists()
        {
            var firstMessage = new Message
            {
                Id = Guid.NewGuid(),
                Body = "Body",
                Created = DateTimeOffset.UtcNow,
                ImageUrl = "ImageUrl",
                Type = MessageType.Default,
                ChannelId = _channelId,
                Owner = _member,
                OwnerId = _member.Id,
                Updated = DateTimeOffset.Now,
                AccessibilityStatus = AccessibilityStatus.Present
            };
            await UnitOfWork.MessageRepository.AddMessageAsync(firstMessage);

            var secondMessage = new Message
            {
                Id = Guid.NewGuid(),
                Body = "Body",
                Created = DateTimeOffset.UtcNow + TimeSpan.FromMinutes(1),
                ImageUrl = "ImageUrl",
                Type = MessageType.Default,
                ChannelId = _channelId,
                Owner = _member,
                OwnerId = _member.Id,
                Updated = DateTimeOffset.Now,
                AccessibilityStatus = AccessibilityStatus.Present
            };
            await UnitOfWork.MessageRepository.AddMessageAsync(secondMessage);

            var prevForFirstMessage = await UnitOfWork.MessageRepository.GetPreviousMessageAsync(firstMessage.ChannelId, firstMessage.OwnerId, firstMessage.Created);
            var prevForSecondMessage = await UnitOfWork.MessageRepository.GetPreviousMessageAsync(secondMessage.ChannelId, secondMessage.OwnerId, secondMessage.Created);

            prevForFirstMessage.Should().BeNull();
            prevForSecondMessage.Should().BeEquivalentTo(firstMessage);
        }

        [Fact]
        public async Task GetLastReadMessageAsyncTest_ShouldReturnLastReadMessageIfExists()
        {
            var firstMessage = new Message
            {
                Id = Guid.NewGuid(),
                Body = "Body",
                Created = DateTimeOffset.UtcNow,
                ImageUrl = "test",
                Type = 0,
                ChannelId = _channelId,
                Owner = _member,
                OwnerId = _member.Id,
                AccessibilityStatus = AccessibilityStatus.Present
            };
            await UnitOfWork.MessageRepository.AddMessageAsync(firstMessage);

            var channelMember = new ChannelMember
            {
                ChannelId = _channelId,
                MemberId = _member.Id,
                LastReadMessageId = null
            };
            await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);

            // Have no read messages
            var readMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(_member.Id, _channelId);
            readMessage.Should().BeNull();

            // Mark as read
            await UnitOfWork.ChannelMemberRepository.SetLastReadMessageAsync(_member.Id, _channelId, firstMessage.Id);
            readMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(_member.Id, _channelId);
            readMessage.Should().BeEquivalentTo(firstMessage);

            // Add second message
            var secondMessage = new Message
            {
                Id = Guid.NewGuid(),
                Body = "Body",
                Created = DateTimeOffset.UtcNow,
                ImageUrl = "test",
                Type = 0,
                ChannelId = _channelId,
                Owner = _member,
                OwnerId = _member.Id,
                AccessibilityStatus = AccessibilityStatus.Present
            };
            await UnitOfWork.MessageRepository.AddMessageAsync(secondMessage);

            // Mark as read
            await UnitOfWork.ChannelMemberRepository.SetLastReadMessageAsync(_member.Id, _channelId, secondMessage.Id);
            readMessage = await UnitOfWork.MessageRepository.GetLastReadMessageAsync(_member.Id, _channelId);
            readMessage.Should().BeEquivalentTo(secondMessage);
        }

        [Fact]
        public async Task AddMessageAsyncTest_ShouldAddMessage()
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                Body = "Body",
                Created = DateTimeOffset.UtcNow,
                ImageUrl = "ImageUrl",
                Type = MessageType.Default,
                ChannelId = _channelId,
                Owner = _member,
                OwnerId = _member.Id,
                Updated = DateTimeOffset.Now,
                AccessibilityStatus = AccessibilityStatus.Present
            };
            await UnitOfWork.MessageRepository.AddMessageAsync(message);

            var addedMessage = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(message.Id);

            addedMessage.Should().BeEquivalentTo(message);
        }

        [Fact]
        public async Task DeleteMessageAsyncTest_ShouldDeleteMessage()
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                Body = "Body",
                Created = DateTimeOffset.UtcNow,
                ImageUrl = "ImageUrl",
                Type = MessageType.Default,
                ChannelId = _channelId,
                Owner = _member,
                OwnerId = _member.Id,
                Updated = DateTimeOffset.Now,
                AccessibilityStatus = AccessibilityStatus.Present
            };
            await UnitOfWork.MessageRepository.AddMessageAsync(message);

            await UnitOfWork.MessageRepository.ArchiveMessageAsync(message.Id);

            var deletedMessage = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(message.Id);
            deletedMessage.Should().BeNull();
        }

        [Fact]
        public async Task UpdateMessageBodyAsync_ShouldUpdateBodyAndUpdatedFields()
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                Body = "Body",
                Created = DateTimeOffset.UtcNow,
                ImageUrl = "ImageUrl",
                Type = MessageType.Default,
                ChannelId = _channelId,
                Owner = _member,
                OwnerId = _member.Id,
                Updated = null,
                AccessibilityStatus = AccessibilityStatus.Present
            };
            await UnitOfWork.MessageRepository.AddMessageAsync(message);

            message.Body = "Updated";
            message.Updated = DateTimeOffset.Now;

            await UnitOfWork.MessageRepository.UpdateMessageBodyAsync(message.Id, message.Body, message.Updated.Value);

            var updatedMessage = await UnitOfWork.MessageRepository.GetMessageWithOwnerAndForwardMessageAsync(message.Id);
            updatedMessage.Should().BeEquivalentTo(message);
        }

        [Fact]
        public async Task GetChannelMessagesCountAsync_ShouldReturnCount()
        {
            const int expectedCount = 5;
            for (var i = 0; i < expectedCount; i++)
            {
                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    Body = "Body",
                    Created = DateTimeOffset.UtcNow,
                    ImageUrl = "ImageUrl",
                    Type = MessageType.Default,
                    ChannelId = _channelId,
                    Owner = _member,
                    OwnerId = _member.Id,
                    Updated = DateTimeOffset.Now,
                    AccessibilityStatus = AccessibilityStatus.Present
                };
                await UnitOfWork.MessageRepository.AddMessageAsync(message);
            }

            var count = await UnitOfWork.MessageRepository.GetChannelMessagesCountAsync(_channelId);

            count.Should().Be(expectedCount);
        }

        [Theory]
        [InlineData(1, "test1")]
        [InlineData(1, "TEST1")]
        [InlineData(5, "test")]
        [InlineData(0, "nonExisting")]
        [InlineData(1, "[]test1")]
        public async Task SearchMessagesInChannelAsync_ShouldReturnFoundMessageIds(int messagesCount, string phrase)
        {
            for (var i = 0; i < 5; i++)
            {
                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    Body = $"[]test{i}",
                    Created = DateTimeOffset.UtcNow,
                    Type = 0,
                    ChannelId = _channelId,
                    AccessibilityStatus = AccessibilityStatus.Present
                };
                await UnitOfWork.MessageRepository.AddMessageAsync(message);
            }

            var searchMessagesCount = await UnitOfWork.MessageRepository.FindMessageIdsAsync(_channelId, phrase);

            searchMessagesCount.Count.Should().Be(messagesCount, $"there is {messagesCount} messages with text: {phrase}");
        }
    }
}
