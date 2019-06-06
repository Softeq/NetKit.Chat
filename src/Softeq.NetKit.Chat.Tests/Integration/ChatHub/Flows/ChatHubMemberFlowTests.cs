// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Softeq.NetKit.Chat.Client.SDK.Enums;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Message;
using Softeq.NetKit.Chat.Client.SDK.Models.SignalRModels.Client;
using Xunit;

namespace Softeq.NetKit.Chat.Tests.Integration.ChatHub.Flows
{
    [TestCaseOrderer("Softeq.NetKit.Chat.Tests.Integration.Utility.AlphabeticalOrderer", "Softeq.NetKit.Chat.Tests")]
    public class ChatHubMemberFlowTests : ChatHubTestBase, IClassFixture<ChatHubFixture>
    {
        private const string RequestUri = "/api/me/channel";

        private readonly TestServer _server;
        private static SignalRClient _adminSignalRClient;
        private static ChannelSummaryResponse _testChannel;
        private static ClientResponse _admin;
        private static string _adminToken;

        public ChatHubMemberFlowTests(ChatHubFixture chatHubFixture)
            : base(chatHubFixture.Configuration)
        {
            _server = chatHubFixture.Server;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Step1_ShouldConnectAdminAndUserClients()
        {
            _adminSignalRClient = new SignalRClient(_server.BaseAddress.ToString());
            _adminToken = await GetJwtTokenAsync("admin@test.test", "123QWqw!");
            await _adminSignalRClient.ConnectAsync(_adminToken, _server.CreateHandler());
        }

        [Fact]
        [Trait("Category", "Integration")]
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
                    _admin.MemberId.ToString(),
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
        public async Task Step3_AddInviteLeaveMember()
        {
            // Invite member
            // Arrange
            var client = _server.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _adminToken);

            var newMemberSignalRClient = new SignalRClient(_server.BaseAddress.ToString());
            var userToken = await GetJwtTokenAsync("user@test.test", "123QWqw!");
            await newMemberSignalRClient.ConnectAsync(userToken, _server.CreateHandler());
            var newUser = await newMemberSignalRClient.AddClientAsync();

            var inviteMultipleMembersRequest = new SignalR.TransportModels.Request.Member.InviteMultipleMembersRequest
            {
                ChannelId = _testChannel.Id,
                RequestId = "A372A27B-4860-44AB-9915-27B8CAFB68A3",
                InvitedMembersIds = new List<Guid> { new Guid(newUser.MemberId.ToString()) }
            };

            // Subscribe event
            MessageResponse userExistsaddedMessageResponse = null;
            void OnMessageAdded(MessageResponse response)
            {
                userExistsaddedMessageResponse = response;
            }
            newMemberSignalRClient.MessageAdded += OnMessageAdded;

            // Act
            async Task<ChannelResponse> GetChannelResponse(HttpClient httpClient)
            {
                var channelResponse = await httpClient.GetAsync(RequestUri);
                var channels = await channelResponse.Content.ReadAsStringAsync();
                var channel = JsonConvert.DeserializeObject<List<ChannelResponse>>(channels).FirstOrDefault();
                return channel;
            }

            await _adminSignalRClient.InviteMultipleMembersAsync(inviteMultipleMembersRequest);

            var twoMemberChannel = await GetChannelResponse(client);

            var addMessageRequest = new AddMessageRequest
            {
                ChannelId = _testChannel.Id,
                Body = "test_body",
                ImageUrl = string.Empty,
                RequestId = "82EEC70D-D808-492C-98E3-6A5B47276990",
                Type = MessageType.Default
            };

            await _adminSignalRClient.AddMessageAsync(addMessageRequest);

            // Unsubscribe events
            newMemberSignalRClient.MessageAdded -= OnMessageAdded;

            // Assert
            userExistsaddedMessageResponse.Should().NotBeNull();
            userExistsaddedMessageResponse.ChannelId.Should().Be(addMessageRequest.ChannelId);
            userExistsaddedMessageResponse.Body.Should().Be(addMessageRequest.Body);
            twoMemberChannel.Should().NotBeNull();
            twoMemberChannel.MembersCount.Should().Be(2);

            // Leave channel
            // Arrange
            var channelRequest = new ChannelRequest
            {
                ChannelId = _testChannel.Id,
                RequestId = "48033741-F54F-46A4-8AB4-3BFF5EACDBAC"
            };

            // Subscribe event
            MessageResponse userDoesntExistaddedMessageResponse = null;
            void OnMessageSent(MessageResponse response)
            {
                userDoesntExistaddedMessageResponse = response;
            }
            newMemberSignalRClient.MessageAdded += OnMessageSent;

            // Act
            await newMemberSignalRClient.LeaveChannelAsync(channelRequest);
            await _adminSignalRClient.AddMessageAsync(addMessageRequest);

            var oneMemberChannel = await GetChannelResponse(client);

            // Unsubscribe events
            newMemberSignalRClient.MessageAdded -= OnMessageSent;

            // Assert
            userDoesntExistaddedMessageResponse.Should().BeNull();
            oneMemberChannel.Should().NotBeNull();
            oneMemberChannel.MembersCount.Should().Be(1);
        }
    }
}
