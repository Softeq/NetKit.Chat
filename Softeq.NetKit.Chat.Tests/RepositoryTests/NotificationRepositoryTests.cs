// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Channel;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Domain.Message;
using Softeq.NetKit.Chat.Domain.Notification;
using Softeq.NetKit.Chat.Tests.Abstract;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.RepositoryTests
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
                Status = UserStatus.Active
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
                Type = MessageType.Notification,
                ChannelId = _channelId,
                OwnerId = _memberId,
                Created = DateTimeOffset.UtcNow
            };
            UnitOfWork.MessageRepository.AddMessageAsync(message).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task AddNotificationAsyncTest()
        {
            // Arrange
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                IsRead = true,
                MessageId = _messageId,
                ChannelId = _channelId,
                MemberId = _memberId
            };
            
            // Act
            await UnitOfWork.NotificationRepository.AddNotificationAsync(notification);
            var newNotification = await UnitOfWork.NotificationRepository.GetNotificationByIdAsync(notification.Id);

            // Assert
            Assert.NotNull(newNotification);
            Assert.Equal(notification.Id, newNotification.Id);
            Assert.Equal(notification.IsRead, newNotification.IsRead);
            Assert.Equal(notification.MessageId, newNotification.MessageId);
            Assert.Equal(notification.MemberId, newNotification.MemberId);
            Assert.Equal(notification.ChannelId, newNotification.ChannelId);
        }

        [Fact]
        public async Task DeleteNotificationAsyncTest()
        {
            // Arrange
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                IsRead = true,
                MessageId = _messageId,
                ChannelId = _channelId,
                MemberId = _memberId
            };

            // Act
            await UnitOfWork.NotificationRepository.AddNotificationAsync(notification);
            await UnitOfWork.NotificationRepository.DeletNotificationAsync(notification.Id);
            var newNotification = await UnitOfWork.NotificationRepository.GetNotificationByIdAsync(notification.Id);

            // Assert
            Assert.Null(newNotification);
        }

        [Fact]
        public async Task GetNotificationByIdAsync()
        {
            // Arrange
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                IsRead = true,
                MessageId = _messageId,
                ChannelId = _channelId,
                MemberId = _memberId
            };
            
            // Act
            await UnitOfWork.NotificationRepository.AddNotificationAsync(notification);
            var newNotification = await UnitOfWork.NotificationRepository.GetNotificationByIdAsync(notification.Id);

            // Assert
            Assert.NotNull(newNotification);
            Assert.Equal(notification.Id, newNotification.Id);
            Assert.Equal(notification.IsRead, newNotification.IsRead);
            Assert.Equal(notification.MessageId, newNotification.MessageId);
            Assert.Equal(notification.MemberId, newNotification.MemberId);
            Assert.Equal(notification.ChannelId, newNotification.ChannelId);
        }

        [Fact]
        public async Task GetMemberNotificaitonsAsyncTest()
        {
            // Arrange
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                IsRead = true,
                MessageId = _messageId,
                ChannelId = _channelId,
                MemberId = _memberId
            };

            // Act
            var notifications = await UnitOfWork.NotificationRepository.GetMemberNotificationsAsync(notification.MemberId);
            await UnitOfWork.NotificationRepository.AddNotificationAsync(notification);
            var newNotifications = await UnitOfWork.NotificationRepository.GetMemberNotificationsAsync(notification.MemberId);

            // Assert
            Assert.NotNull(notifications);
            Assert.Empty(notifications);
            Assert.NotNull(newNotifications);
            Assert.NotEmpty(newNotifications);
            Assert.True(newNotifications.Count > notifications.Count);
        }
    }
}