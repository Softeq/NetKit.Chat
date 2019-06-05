// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Channel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Client.SDK.Enums;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.ChatHub.Flows
{
    [TestCaseOrderer("Softeq.NetKit.Chat.Tests.Integration.Utility.AlphabeticalOrderer", "Softeq.NetKit.Chat.Tests")]
    public class ChatHubChannelFlowTests : ChatHubTestBase, IClassFixture<ChatHubFixture>
    {
        private readonly TestServer _server;
        private static SignalRClient _adminSignalRClient;
        private static SignalRClient _userSignalRClient;
        private static ChannelSummaryResponse _testChannel;

        public ChatHubChannelFlowTests(ChatHubFixture chatHubFixture)
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
            _userSignalRClient.ValidationFailed += (errors, requestId) => throw new Exception($"Errors: {errors}{Environment.NewLine}RequestId: {requestId}");
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Step2_ShouldCreateChannel()
        {
            // Arrange
            var admin = await _adminSignalRClient.AddClientAsync();
            var client = await _userSignalRClient.AddClientAsync();

            // Subscribe ChannelCreated event
            ChannelSummaryResponse createdChannel = null;
            void OnChannelCreated(ChannelSummaryResponse channelSummaryResponse)
            {
                createdChannel = channelSummaryResponse;
            }
            _adminSignalRClient.ChannelCreated += OnChannelCreated;

            // Subscribe MemberJoined event
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
                RequestId = "3433E3F8-E363-4A07-8CAA-8F759340F769",
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
        public async Task Step3_ShouldUpdateChannel()
        {
            // Arrange
            var updateChannelRequest = new UpdateChannelRequest
            {
                ChannelId = _testChannel.Id,
                Name = $"new_{_testChannel.Name}",
                Description = $"new_{_testChannel.Description}",
                RequestId = "EA701C57-477D-42E3-B660-E510F7F8C72F",
                WelcomeMessage = $"new_{_testChannel.WelcomeMessage}"
            };

            // Subscribe event
            ChannelSummaryResponse channelSummaryResponse = null;
            void OnChannelUpdated(ChannelSummaryResponse response)
            {
                channelSummaryResponse = response;
            }
            _userSignalRClient.ChannelUpdated += OnChannelUpdated;

            // Act
            await _adminSignalRClient.UpdateChannelAsync(updateChannelRequest);

            // Unsubscribe events
            _userSignalRClient.ChannelUpdated -= OnChannelUpdated;

            // Assert
            channelSummaryResponse.Id.Should().Be(updateChannelRequest.ChannelId);
            channelSummaryResponse.Description.Should().BeEquivalentTo(updateChannelRequest.Description);
            channelSummaryResponse.Name.Should().BeEquivalentTo(updateChannelRequest.Name);
            channelSummaryResponse.WelcomeMessage.Should().BeEquivalentTo(updateChannelRequest.WelcomeMessage);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Step4_ShouldCloseChannel()
        {
            // Arrange
            var channelRequest = new ChannelRequest
            {
                RequestId = "623AE57B-9917-4DED-BFFC-44F09C906F10",
                ChannelId = _testChannel.Id
            };

            // Subscribe event
            ChannelSummaryResponse channelSummaryResponse = null;
            void OnChannelClosed(ChannelSummaryResponse response)
            {
                channelSummaryResponse = response;
            }
            _userSignalRClient.ChannelClosed += OnChannelClosed;

            // Act
            await _adminSignalRClient.CloseChannelAsync(channelRequest);

            // Unsubscribe events
            _userSignalRClient.ChannelClosed -= OnChannelClosed;

            // Assert
            channelSummaryResponse.IsClosed.Should().BeTrue();
        }
    }
}
