// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Softeq.NetKit.Chat.Domain.DomainModels;
using System;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.RepositoryTests
{
    public class DirectMessagesRepositoryTests : BaseTest
    {
        [Fact]
        public void ShouldAddMessage()
        {
            // Arrange
            var directChannelId = new Guid("9EDBC8A4-2EEC-4FB2-8410-A2852EB8989A");
            var ownerId = new Guid("2C1CFFE1-3656-4100-9364-6D100D006FA0");
            var memberId = new Guid("B2C9F384-B0E1-45BE-B729-4DB1BB44FDBF");

            var owner = new Member
            {
                Id = ownerId,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            var member = new Member
            {
                Id = memberId,
                Email = "test",
                Role = UserRole.User,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            UnitOfWork.MemberRepository.AddMemberAsync(owner).GetAwaiter().GetResult();
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();

            UnitOfWork.DirectChannelRepository.CreateDirectChannel(directChannelId, ownerId, memberId).GetAwaiter().GetResult();

            var datetime = DateTimeOffset.UtcNow;
            var messageId = new Guid("51F70B32-B656-4FF8-9C6C-69252F2295B9");
            var directMessage = new DirectMessage
            {
                Body = "TestBody",
                Created = datetime,
                DirectChannelId = directChannelId,
                Id = messageId,
                OwnerId = ownerId,
                Updated = datetime
            };

            // Act
            UnitOfWork.DirectMessagesRepository.AddMessageAsync(directMessage);
            var result = UnitOfWork.DirectMessagesRepository.GetMessagesByIdAsync(directMessage.Id).GetAwaiter().GetResult();

            // Assert
            result.Should().BeEquivalentTo(directMessage);
        }

        [Fact]
        public void ShouldDeleteMessage()
        {
            // Arrange
            var directChannelId = new Guid("9EDBC8A4-2EEC-4FB2-8410-A2852EB8989A");
            var ownerId = new Guid("2C1CFFE1-3656-4100-9364-6D100D006FA0");
            var memberId = new Guid("B2C9F384-B0E1-45BE-B729-4DB1BB44FDBF");

            var owner = new Member
            {
                Id = ownerId,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            var member = new Member
            {
                Id = memberId,
                Email = "test",
                Role = UserRole.User,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            UnitOfWork.MemberRepository.AddMemberAsync(owner).GetAwaiter().GetResult();
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();
            UnitOfWork.DirectChannelRepository.CreateDirectChannel(directChannelId, ownerId, memberId).GetAwaiter().GetResult();

            var directMessageDatetime = DateTimeOffset.UtcNow;
            var directMessageId = new Guid("51F70B32-B656-4FF8-9C6C-69252F2295B9");
            var directMessage = new DirectMessage
            {
                Body = "TestBody",
                Created = directMessageDatetime,
                DirectChannelId = directChannelId,
                Id = directMessageId,
                OwnerId = ownerId,
                Updated = directMessageDatetime
            };

            // Act
            UnitOfWork.DirectMessagesRepository.AddMessageAsync(directMessage);
            UnitOfWork.DirectMessagesRepository.DeleteMessageAsync(directMessageId).GetAwaiter();

            var result = UnitOfWork.DirectMessagesRepository.GetMessagesByIdAsync(directMessageId).GetAwaiter().GetResult();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ShouldUpdateMessage()
        {
            // Arrange
            var directChannelId = new Guid("9EDBC8A4-2EEC-4FB2-8410-A2852EB8989A");
            var ownerId = new Guid("2C1CFFE1-3656-4100-9364-6D100D006FA0");
            var memberId = new Guid("B2C9F384-B0E1-45BE-B729-4DB1BB44FDBF");

            var owner = new Member
            {
                Id = ownerId,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            var member = new Member
            {
                Id = memberId,
                Email = "test",
                Role = UserRole.User,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            UnitOfWork.MemberRepository.AddMemberAsync(owner).GetAwaiter().GetResult();
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();
            UnitOfWork.DirectChannelRepository.CreateDirectChannel(directChannelId, ownerId, memberId).GetAwaiter().GetResult();

            var directMessageDatetime = DateTimeOffset.UtcNow;
            var directMessageId = new Guid("51F70B32-B656-4FF8-9C6C-69252F2295B9");
            var directMessage = new DirectMessage
            {
                Body = "TestBody",
                Created = directMessageDatetime,
                DirectChannelId = directChannelId,
                Id = directMessageId,
                OwnerId = ownerId,
                Updated = directMessageDatetime
            };

            UnitOfWork.DirectMessagesRepository.AddMessageAsync(directMessage);

            var newDirectMessageDatetime = DateTimeOffset.UtcNow;
            var message = UnitOfWork.DirectMessagesRepository.GetMessagesByIdAsync(directMessageId).GetAwaiter().GetResult();
            message.Body = "NewTestBody";
            message.Updated = newDirectMessageDatetime;

            UnitOfWork.DirectMessagesRepository.UpdateMessageAsync(message).GetAwaiter().GetResult();

            // Act
            var result = UnitOfWork.DirectMessagesRepository.GetMessagesByIdAsync(message.Id).GetAwaiter().GetResult();

            // Assert
            result.Body.Should().Be(message.Body);
            result.Updated.Should().Be(newDirectMessageDatetime);
            result.Id.Should().Be(directMessageId);
        }

        [Fact]
        public void ShouldReturnMessagesOfChannel()
        {
            // Arrange
            var directChannelId = new Guid("9EDBC8A4-2EEC-4FB2-8410-A2852EB8989A");
            var ownerId = new Guid("2C1CFFE1-3656-4100-9364-6D100D006FA0");
            var memberId = new Guid("B2C9F384-B0E1-45BE-B729-4DB1BB44FDBF");

            var owner = new Member
            {
                Id = ownerId,
                Email = "test",
                Role = UserRole.Admin,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            var member = new Member
            {
                Id = memberId,
                Email = "test",
                Role = UserRole.User,
                IsAfk = true,
                IsBanned = true,
                LastNudged = DateTimeOffset.UtcNow,
                LastActivity = DateTimeOffset.UtcNow,
                Name = "test",
                SaasUserId = "test",
                Status = UserStatus.Offline
            };

            UnitOfWork.MemberRepository.AddMemberAsync(owner).GetAwaiter().GetResult();
            UnitOfWork.MemberRepository.AddMemberAsync(member).GetAwaiter().GetResult();
            UnitOfWork.DirectChannelRepository.CreateDirectChannel(directChannelId, ownerId, memberId).GetAwaiter().GetResult();

            var directMessage01 = new DirectMessage
            {
                Body = "TestBody01",
                Created = DateTimeOffset.UtcNow,
                DirectChannelId = directChannelId,
                Id = new Guid("661F903F-246F-434A-AD7A-A5ED76C5919A"),
                OwnerId = ownerId,
                Updated = DateTimeOffset.UtcNow
            };

            var directMessage02 = new DirectMessage
            {
                Body = "TestBody02",
                Created = DateTimeOffset.UtcNow,
                DirectChannelId = directChannelId,
                Id = new Guid("6F01ABA0-6DD4-49BC-A6DD-83350E7F6D74"),
                OwnerId = ownerId,
                Updated = DateTimeOffset.UtcNow
            };

            var directMessage03 = new DirectMessage
            {
                Body = "TestBody03",
                Created = DateTimeOffset.UtcNow,
                DirectChannelId = directChannelId,
                Id = new Guid("7AF65CEC-788B-4040-9590-96793615D9FD"),
                OwnerId = memberId,
                Updated = DateTimeOffset.UtcNow
            };

            var directMessage04 = new DirectMessage
            {
                Body = "TestBody04",
                Created = DateTimeOffset.UtcNow,
                DirectChannelId = directChannelId,
                Id = new Guid("094630E1-95FC-4B9A-ABF9-D89361C44C07"),
                OwnerId = ownerId,
                Updated = DateTimeOffset.UtcNow
            };

            // Act
            UnitOfWork.DirectMessagesRepository.AddMessageAsync(directMessage01);
            UnitOfWork.DirectMessagesRepository.AddMessageAsync(directMessage02);
            UnitOfWork.DirectMessagesRepository.AddMessageAsync(directMessage03);
            UnitOfWork.DirectMessagesRepository.AddMessageAsync(directMessage04);

            var result = UnitOfWork.DirectMessagesRepository.GetMessagesByChannelIdAsync(directChannelId).GetAwaiter().GetResult();

            // Assert
            result.Count.Should().Be(4);
            result[1].Body.Should().Be(directMessage03.Body);
            result[3].Body.Should().Be(directMessage01.Body);
        }
    }
}
