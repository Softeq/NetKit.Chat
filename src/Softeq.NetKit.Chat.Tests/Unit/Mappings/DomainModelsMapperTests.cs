// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Autofac;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Chat.Data.Cloud.Azure;
using Softeq.NetKit.Chat.Data.Persistent.Sql;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Services;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Web;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Unit.Mappings
{
    public class DomainModelsMapperTests
    {
        private readonly IDomainModelsMapper _domainModelsMapper;

        public DomainModelsMapperTests()
        {
            var builder = new ContainerBuilder();
            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            builder.RegisterInstance(configurationRoot)
                .As<IConfigurationRoot>()
                .As<IConfiguration>();

            builder.RegisterModule<DataPersistentSqlDiModule>();
            builder.RegisterModule<DataCloudAzureDiModule>();
            builder.RegisterModule<DomainServicesDiModule>();
            builder.RegisterModule<WebDiModule>();

            var lifetimeScope = builder.Build();

            _domainModelsMapper = lifetimeScope.Resolve<IDomainModelsMapper>();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void MapToMessageResponse_ShouldMapMessageToMessageResponse()
        {
            var message = new Message
            {
                Owner = new Member
                {
                    Name = "member name"
                }
            };

            var response = _domainModelsMapper.MapToMessageResponse(message);

            response.Sender.UserName.Should().BeEquivalentTo(message.Owner.Name);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void MapToMemberSummaryResponse_ShouldMapMemberToMemberSummaryResponse()
        {
            var member = new Member
            {
                PhotoName = "photoName",
                Name = "Name"
            };
            var summary = _domainModelsMapper.MapToMemberSummaryResponse(member);

            summary.AvatarUrl.Should().Contain(member.PhotoName);
            summary.UserName.Should().Be(member.Name);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void MapToChannelSummaryResponse_ShouldMapChannelAndChannelMember_WithMessages_ToChannelSummaryResponse()
        {
            // Arrange
            var lastReadMessage = new Message
            {
                Body = "last read message",
                Created = DateTimeOffset.UtcNow
            };
            var oldMessage = new Message
            {
                Body = "old message",
                Created = lastReadMessage.Created.AddMinutes(-1)
            };
            var lastChannelMessage = new Message
            {
                Body = "last channel message",
                Created = lastReadMessage.Created.AddMinutes(1)
            };

            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                Created = DateTimeOffset.UtcNow.AddMinutes(-10),
                Updated = DateTimeOffset.UtcNow.AddMinutes(-5),
                Messages = new List<Message>
                {
                    oldMessage,
                    lastReadMessage,
                    lastChannelMessage
                },
                Name = "channel name",
                IsClosed = true,
                CreatorId = Guid.NewGuid(),
                Creator = new Member
                {
                    Id = Guid.NewGuid(),
                    SaasUserId = "63975AE7-A5B2-477A-9163-72191B7793A4"
                },
                Description = "Description",
                WelcomeMessage = "WelcomeMessage",
                Type = ChannelType.Private,
                PhotoUrl = "PhotoUrl"
            };

            var channelMember = new ChannelMember
            {
                IsMuted = true,
                IsPinned = true
            };

            // Act
            var response = _domainModelsMapper.MapToChannelSummaryResponse(channel, channelMember, lastReadMessage);

            // Assert
            response.IsMuted.Should().Be(channelMember.IsMuted);
            response.IsPinned.Should().Be(channelMember.IsPinned);

            response.Id.Should().Be(channel.Id);
            response.Created.Should().Be(channel.Created);
            response.Updated.Should().Be(channel.Updated);
            response.Name.Should().Be(channel.Name);
            response.IsClosed.Should().Be(channel.IsClosed);
            response.CreatorId.Should().Be(channel.CreatorId);
            response.CreatorSaasUserId.Should().Be(channel.Creator.SaasUserId);
            response.Description.Should().Be(channel.Description);
            response.WelcomeMessage.Should().Be(channel.WelcomeMessage);
            response.Type.Should().Be(channel.Type);
            response.PhotoUrl.Should().Be(channel.PhotoUrl);

            response.UnreadMessagesCount.Should().Be(1);
            response.LastMessage.Body.Should().Be(lastChannelMessage.Body);
            response.LastMessage.IsRead.Should().Be(false);
            response.Creator.Id.Should().Be(channel.Creator.Id);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void MapToChannelSummaryResponse_ShouldSetLastMessageNULL_WithoutMessages()
        {
            // Arrange
            var channel = new Channel
            {
                Messages = new List<Message>()
            };

            var channelMember = new ChannelMember();

            var lastReadMessage = new Message { Created = DateTimeOffset.UtcNow };

            // Act
            var response = _domainModelsMapper.MapToChannelSummaryResponse(channel, channelMember, lastReadMessage);

            // Assert
            response.UnreadMessagesCount.Should().Be(0);
            response.LastMessage.Should().Be(null);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void MapToClientResponse_ShouldMapClientToClientResponse()
        {
            var client = new Client
            {
                ClientConnectionId = "2A4C5F69-0464-4F6C-97F4-7E6D8FF93CA8",
                Member = new Member
                {
                    SaasUserId = "DD507B44-5DFE-4CFD-BD83-34A850150C9D"
                },
                Name = "Name"
            };

            var response = _domainModelsMapper.MapToClientResponse(client);

            response.ConnectionClientId.Should().Be(client.ClientConnectionId);
            response.UserName.Should().Be(client.Name);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void MapToDirectChannelResponse_ShouldMapTwoMembersToDirectChannelResponse()
        {
            // Arrange
            var channelId = new Guid("6CCC3DD2-826C-4523-AB2C-A3839BB166CB");

            var owner = new Member
            {
                PhotoName = "firstPhotoName",
                Name = "FirstName"
            };

            var member = new Member
            {
                PhotoName = "secondPhotoName",
                Name = "SecondName"
            };

            // Act
            var response = _domainModelsMapper.MapToDirectChannelResponse(channelId, owner, member);

            // Assert
            response.DirectChannelId.Should().Be(channelId);
            response.Owner.AvatarUrl.Should().Contain(owner.PhotoName);
            response.Member.AvatarUrl.Should().Contain(member.PhotoName);
            response.Owner.UserName.Should().Be(owner.Name);
            response.Member.UserName.Should().Be(member.Name);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void MapToNotificationSettingsResponse_ShouldMapNotificationSettingsResponse()
        {
            var notificationSettings = new NotificationSettings
            {
                MemberId = Guid.Parse("2A4C5F69-0464-4F6C-97F4-7E6D8FF93CA8"),
                IsChannelNotificationsDisabled = NotificationSettingValue.Enabled,
                Id = Guid.Parse("DD507B44-5DFE-4CFD-BD83-34A850150C9D")
            };

            var response = _domainModelsMapper.MapToNotificationSettingsResponse(notificationSettings);

            response.IsChannelNotificationsDisabled.Should().Be(notificationSettings.IsChannelNotificationsDisabled);
            response.MemberId.Should().Be(notificationSettings.MemberId);
        }
    }
}