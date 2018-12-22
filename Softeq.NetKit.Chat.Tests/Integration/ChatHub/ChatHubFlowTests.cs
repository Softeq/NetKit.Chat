// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Channel;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.ChatHub
{
    [TestCaseOrderer("Softeq.NetKit.Chat.Tests.Integration.Utility.AlphabeticalOrderer", "Softeq.NetKit.Chat.Tests")]
    public class ChatHubFlowTests : ChatHubTestBase, IClassFixture<ChatHubFixture>
    {
        private readonly TestServer _server;

        private static SignalRClient _adminSignalRClient;
        private static SignalRClient _userSignalRClient;

        private static ChannelSummaryResponse _testChannel;

        public ChatHubFlowTests(ChatHubFixture chatHubFixture)
            : base(chatHubFixture.Configuration)
        {
            _server = chatHubFixture.Server;
        }

        [Fact]
        public async Task Step1_ShouldConnectAdminAndUserClients()
        {
            // TODO:
            // I'm not sure we need to create handlers for each client.
            // If so, create `var messageHandler = _server.CreateHandler();`
            // and use it in both `_...SignalRClient.ConnectAsync(...Token, messageHandler)
            // If not - delete this comment.

            _adminSignalRClient = new SignalRClient(_server.BaseAddress.ToString());
            var adminToken = await GetJwtTokenAsync("admin@test.test", "123QWqw!");
            await _adminSignalRClient.ConnectAsync(adminToken, _server.CreateHandler());
            _adminSignalRClient.ValidationFailed += (errors, requestId) =>
            {
                // TODO:
                // It handles, but exception do not stops the tests.
                // Find solution to handle exceptions and validation errors that could break the tests.
                throw new Exception($"Errors: {errors}{Environment.NewLine}RequestId: {requestId}");
            };

            //_userSignalRClient = new SignalRClient(_server.BaseAddress.ToString());
            // TODO: find out and replace ???????????????? by user password
            //var userToken = await GetJwtTokenAsync("user@test.test", "????????????????");
            //await _userSignalRClient.ConnectAsync(userToken, _server.CreateHandler());
            //_userSignalRClient.ValidationFailed += (errors, requestId) => throw new Exception($"Errors: {errors}{Environment.NewLine}RequestId: {requestId}");
        }

        [Fact]
        public async Task Step2_ShouldCreateChannel()
        {
            // Arrange
            var adminClient = await _adminSignalRClient.AddClientAsync();

            // Subscribe ChannelCreated event
            ChannelSummaryResponse createdChannel = null;
            void OnChannelCreated(ChannelSummaryResponse channelSummaryResponse)
            {
                createdChannel = channelSummaryResponse;
            }
            _adminSignalRClient.ChannelCreated += OnChannelCreated;

            // Subscribe MemberJoined event
            MemberSummary joinedMember = null;
            ChannelSummaryResponse joinedChannel = null;
            void OnMemberJoined(MemberSummary memberSummary, ChannelSummaryResponse channelSummaryResponse)
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
                Type = ChannelType.Public,
                RequestId = "3433E3F8-E363-4A07-8CAA-8F759340F769"
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
        // TODO: Maybe should be another test, discuss with client-app developers
        public async Task Step3_UserShouldJoinChannel()
        {
            //_testChannel
        }
    }
}