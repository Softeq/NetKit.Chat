// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Channel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.ChatHub.Flows
{
    [TestCaseOrderer("Softeq.NetKit.Chat.Tests.Integration.Utility.AlphabeticalOrderer", "Softeq.NetKit.Chat.Tests")]
    public class ChatHubMemberFlowTests : ChatHubTestBase, IClassFixture<ChatHubFixture>
    {
        private readonly TestServer _server;

        private static SignalRClient _adminSignalRClient;
        private static SignalRClient _userSignalRClient;

        private static ChannelSummaryResponse _testChannel;

        private static ClientResponse _admin;

        public ChatHubMemberFlowTests(ChatHubFixture chatHubFixture)
            : base(chatHubFixture.Configuration)
        {
            _server = chatHubFixture.Server;
        }

        [Fact]
        public async Task Step1_ShouldConnectAdminAndUserClients()
        {
            _adminSignalRClient = new SignalRClient(_server.BaseAddress.ToString());
            var adminToken = await GetJwtTokenAsync("admin@test.test", "123QWqw!");
            await _adminSignalRClient.ConnectAsync(adminToken, _server.CreateHandler());

            _userSignalRClient = new SignalRClient(_server.BaseAddress.ToString());
            var userToken = await GetJwtTokenAsync("user@test.test", "123QWqw!");
            await _userSignalRClient.ConnectAsync(userToken, _server.CreateHandler());
            _userSignalRClient.ValidationFailed += (errors, requestId) => throw new Exception($"Errors: {errors}{Environment.NewLine}RequestId: {requestId}");
        }

        [Fact]
        public async Task Step2_ShouldCreateChannel()
        {
            // Arrange
            _admin = await _adminSignalRClient.AddClientAsync();

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
                Type = ChannelType.Private,
                RequestId = "3433E3F8-E363-4A07-8CAA-8F759340F769",
                AllowedMembers = new List<string>
                {
                    _admin.SaasUserId,
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
        public async Task Step3_AddInviteUpdateMember()
        {
            // Add member
            // Arrange
            var newUser = await _userSignalRClient.AddClientAsync();

            var inviteMultipleMembersRequest = new SignalR.TransportModels.Request.Member.InviteMultipleMembersRequest
            {
                ChannelId = _testChannel.Id,
                RequestId = "A372A27B-4860-44AB-9915-27B8CAFB68A3",
                InvitedMembersIds = new List<Guid> { new Guid(newUser.SaasUserId) }
            };



            // Subscribe event

            // Act
            await _adminSignalRClient.InviteMultipleMembersAsync(inviteMultipleMembersRequest);

            // Unsubscribe events

            // Assert

        }
    }
}
