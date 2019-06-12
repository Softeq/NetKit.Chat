// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.TransportModels.Enums;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Channel;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Request.Message;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Member;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Message;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.ChatHub.Flows
{
    [TestCaseOrderer("Softeq.NetKit.Chat.Tests.Integration.Utility.AlphabeticalOrderer", "Softeq.NetKit.Chat.Tests")]
    public class ChatHubMessagesFlowTests : ChatHubTestBase, IClassFixture<ChatHubFixture>
    {
        private readonly TestServer _server;
        private static SignalRClient _adminSignalRClient;
        private static SignalRClient _userSignalRClient;
        private static ChannelSummaryResponse _testChannel;

        public ChatHubMessagesFlowTests(ChatHubFixture chatHubFixture)
            : base(chatHubFixture.Configuration)
        {
            _server = chatHubFixture.Server;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Step1_ShouldConnectAdminAndUserClients()
        {
            _adminSignalRClient = new SignalRClient(_server.BaseAddress.ToString());
            var adminToken = await GetJwtTokenAsync("admin@test.test", "123QWqw!");
            await _adminSignalRClient.ConnectAsync(adminToken, _server.CreateHandler());

            _adminSignalRClient.ValidationFailed += (errors, requestId) =>
            {
                throw new Exception($"Errors: {errors}{Environment.NewLine}RequestId: {requestId}");
            };

            _userSignalRClient = new SignalRClient(_server.BaseAddress.ToString());
            var userToken = await GetJwtTokenAsync("user@test.test", "123QWqw!");
            await _userSignalRClient.ConnectAsync(userToken, _server.CreateHandler());
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Step2_ShouldCreateChannel()
        {
            // Arrange
            var admin = await _adminSignalRClient.AddClientAsync();
            var client = await _userSignalRClient.AddClientAsync();

            // Subscribe event
            ChannelSummaryResponse createdChannel = null;
            void OnChannelCreated(ChannelSummaryResponse channelSummaryResponse)
            {
                createdChannel = channelSummaryResponse;
            }
            _adminSignalRClient.ChannelCreated += OnChannelCreated;

            // Subscribe event
            MemberSummaryResponse joinedMember = null;
            ChannelSummaryResponse joinedChannel = null;
            void OnMemberJoined(MemberSummaryResponse memberSummary, ChannelSummaryResponse channelSummaryResponse)
            {
                joinedMember = memberSummary;
                joinedChannel = channelSummaryResponse;
            }
            _adminSignalRClient.MemberJoined += OnMemberJoined;

            var createChannelRequest = new CreateChannelRequest
            {
                Name = "channel_name_without_spaces",
                Description = "channel description",
                WelcomeMessage = "welcome message",
                Type = ChannelType.Private,
                AllowedMembers = new List<string>
                {
                    admin.MemberId.ToString(),
                    client.MemberId.ToString()
                }
            };

            // Act
            _testChannel = await _adminSignalRClient.CreateChannelAsync(createChannelRequest);

            // Unsubscribe events
            _adminSignalRClient.ChannelCreated -= OnChannelCreated;
            _adminSignalRClient.MemberJoined -= OnMemberJoined;

            // Assert
            createdChannel.Should().NotBeNull();
            createdChannel.Should().BeEquivalentTo(_testChannel);

            joinedMember.Should().NotBeNull();

            joinedChannel.Should().NotBeNull();
            joinedChannel.Should().BeEquivalentTo(_testChannel);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Step3_ShouldAddUpdateDeleteMessage()
        {
            // Add message
            // Arrange
            var addMessageRequest = new AddMessageRequest
            {
                ChannelId = _testChannel.Id,
                Body = "test_body",
                ImageUrl = string.Empty,
                Type = MessageType.Default
            };

            // Subscribe event
            MessageResponse addedMessageResponse = null;
            void OnMessageAdded(MessageResponse response)
            {
                addedMessageResponse = response;
            }
            _userSignalRClient.MessageAdded += OnMessageAdded;

            // Act
            var messageResponse = await _adminSignalRClient.AddMessageAsync(addMessageRequest);

            // Unsubscribe events
            _userSignalRClient.MessageAdded -= OnMessageAdded;

            // Assert
            addedMessageResponse.Should().NotBeNull();
            addedMessageResponse.Should().BeEquivalentTo(messageResponse);

            // Update message
            // Arrange
            var updatedMessageRequest = new UpdateMessageRequest
            {
                Body = $"new_{messageResponse.Body}",
                MessageId = messageResponse.Id
            };

            // Subscribe event
            void OnMessageUpdated(MessageResponse response)
            {
                messageResponse = response;
            }
            _userSignalRClient.MessageUpdated += OnMessageUpdated;

            // Act
            await _adminSignalRClient.UpdateMessageAsync(updatedMessageRequest);

            // Unsubscribe events
            _userSignalRClient.MessageUpdated -= OnMessageUpdated;

            // Assert
            messageResponse.Should().NotBeNull();
            messageResponse.Body.Should().BeEquivalentTo(updatedMessageRequest.Body);

            // Delete message
            // Arrange
            var deleteMessageRequest = new DeleteMessageRequest
            {
                MessageId = messageResponse.Id
            };

            // Subscribe event
            ChannelSummaryResponse channelSummaryResponse = null;
            Guid messageId = new Guid();
            void OnMessageDeleted(Guid id, ChannelSummaryResponse response)
            {
                messageId = id;
                channelSummaryResponse = response;
            }
            _userSignalRClient.MessageDeleted += OnMessageDeleted;

            // Act
            await _adminSignalRClient.DeleteMessageAsync(deleteMessageRequest);

            // Unsubscribe events
            _userSignalRClient.MessageDeleted -= OnMessageDeleted;

            // Assert
            messageId.Should().Be(messageResponse.Id);
            channelSummaryResponse.UnreadMessagesCount.Should().Be(0);
        }
    }
}