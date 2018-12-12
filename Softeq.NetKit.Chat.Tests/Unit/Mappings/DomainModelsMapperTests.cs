// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
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
        public void ShouldMapMessageToMessageResponse()
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
        public void ShouldMapMemberToMemberSummary()
        {
            var member = new Member
            {
                PhotoName = "photoName",
                Name = "Name"
            };
            var summary = _domainModelsMapper.MapToMemberSummary(member);

            summary.AvatarUrl.Should().Contain($"/{member.PhotoName}");
            summary.UserName.Should().Be(member.Name);
        }

        [Fact]
        public void ShouldMapChannelAndChannelMemberToChannelSummaryResponse()
        {
            // Arrange
            var lastReadMessage = new Message
            {
                Created = DateTimeOffset.UtcNow
            };

            var channel = new Channel
            {
                Id = Guid.NewGuid(),
                Created = DateTimeOffset.UtcNow.AddMinutes(-10),
                Updated = DateTimeOffset.UtcNow.AddMinutes(-5),
                Messages = new List<Message>
                {
                    new Message
                    {
                        Body = "last message body",
                        Created = lastReadMessage.Created.AddMinutes(1)
                    },
                    lastReadMessage,
                    new Message
                    {
                        Body = "message body",
                        Created = lastReadMessage.Created.AddMinutes(-1)
                    }
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
            response.LastMessage.Body.Should().Be(channel.Messages.First().Body);
            response.Creator.Id.Should().Be(channel.Creator.Id);
        }

        [Fact]
        public void ShouldMapClientToClientResponse()
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
            response.SaasUserId.Should().Be(client.Member.SaasUserId);
            response.UserName.Should().Be(client.Name);
        }
    }
}