// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.RepositoryTests
{
    public class NotificationRepositoryTests : BaseTest
    {
        private readonly Guid _memberId = new Guid("4dce9d78-0104-42c4-a854-7011c26aec4a");
        private readonly Guid _channelId = new Guid("ff3a9353-e293-4225-b8a6-5f47bef347b8");
        private readonly Guid _messageId = new Guid("4c27f501-cd91-4576-960b-956d2dfa1789");

        public NotificationRepositoryTests()
        {
            var member = new Member
            {
                Id = _memberId,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online
            };
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            var channel = new Channel
            {
                Id = _channelId,
                CreatorId = member.Id,
                Name = "testNotificationChannel",
                Type = ChannelType.Public,
                MembersCount = 0
            };
            UnitOfWork.ChannelRepository.AddChannelAsync(channel).GetAwaiter().GetResult();

            var message = new Message
            {
                Id = _messageId,
                Type = MessageType.System,
                ChannelId = _channelId,
                OwnerId = _memberId,
                Created = DateTimeOffset.UtcNow
            };
            UnitOfWork.MessageRepository.AddMessageAsync(message).GetAwaiter().GetResult();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AddNotificationAsync_ShouldAddNotification()
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                IsRead = true,
                MessageId = _messageId,
                ChannelId = _channelId,
                MemberId = _memberId
            };
            
            await UnitOfWork.NotificationRepository.AddNotificationAsync(notification);

            var newNotification = await UnitOfWork.NotificationRepository.GetNotificationByIdAsync(notification.Id);

            newNotification.Should().BeEquivalentTo(notification);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task DeleteNotificationAsync_ShouldDeleteNotification()
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                IsRead = true,
                MessageId = _messageId,
                ChannelId = _channelId,
                MemberId = _memberId
            };
            
            await UnitOfWork.NotificationRepository.AddNotificationAsync(notification);

            await UnitOfWork.NotificationRepository.DeletNotificationAsync(notification.Id);

            var newNotification = await UnitOfWork.NotificationRepository.GetNotificationByIdAsync(notification.Id);

            newNotification.Should().BeNull();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetNotificationByIdAsync_ShouldReturnNotification()
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                IsRead = true,
                MessageId = _messageId,
                ChannelId = _channelId,
                MemberId = _memberId
            };
            
            await UnitOfWork.NotificationRepository.AddNotificationAsync(notification);

            var newNotification = await UnitOfWork.NotificationRepository.GetNotificationByIdAsync(notification.Id);

            newNotification.Should().BeEquivalentTo(notification);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetMemberNotificationsWithMemberMessageAndChannelAsync_ShouldReturnNotificationsWithFilledProperties()
        {
            // Arrange
            var member = new Member
            {
                Id = Guid.NewGuid(),
                Role = UserRole.User,
                IsBanned = true,
                LastActivity = DateTimeOffset.UtcNow,
                Status = UserStatus.Online,
                Email = "Email",
                LastNudged = DateTimeOffset.UtcNow,
                Name = "Name",
                PhotoName = "PhotoName",
                SaasUserId = "SaasUserId"
            };
            await UnitOfWork.MemberRepository.AddMemberAsync(member);

            var expectedNotifications = new List<Notification>();
            for (var i = 0; i < 3; i++)
            {
                var channel = new Channel
                {
                    Id = Guid.NewGuid(),
                    IsClosed = true,
                    CreatorId = member.Id,
                    Created = DateTimeOffset.UtcNow,
                    Name = "Name",
                    Type = ChannelType.Public,
                    Description = "Description",
                    WelcomeMessage = "WelcomeMessage",
                    MembersCount = 10,
                    Updated = DateTimeOffset.Now,
                    PhotoUrl = "PhotoUrl"
                };
                await UnitOfWork.ChannelRepository.AddChannelAsync(channel);

                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    Body = "Body",
                    Created = DateTimeOffset.UtcNow,
                    ImageUrl = "ImageUrl",
                    Type = MessageType.Default,
                    ChannelId = channel.Id,
                    OwnerId = member.Id,
                    Updated = DateTimeOffset.Now
                };
                await UnitOfWork.MessageRepository.AddMessageAsync(message);

                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    IsRead = true,
                    Message = message,
                    MessageId = message.Id,
                    Channel = channel,
                    ChannelId = channel.Id,
                    Member = member,
                    MemberId = member.Id
                };
                await UnitOfWork.NotificationRepository.AddNotificationAsync(notification);
                expectedNotifications.Add(notification);
            }

            var notifications = await UnitOfWork.NotificationRepository.GetMemberNotificationsWithMemberMessageAndChannelAsync(member.Id);

            notifications.Should().BeEquivalentTo(expectedNotifications);
        }
    }
}