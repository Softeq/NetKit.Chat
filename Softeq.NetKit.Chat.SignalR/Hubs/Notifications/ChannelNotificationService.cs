// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public class ChannelNotificationService : BaseNotificationService, IChannelNotificationService
    {
        public ChannelNotificationService(IChannelMemberService channelMemberService, IMemberService memberService, IClientService clientService, IHubContext<ChatHub> hubContext)
            : base(channelMemberService, memberService, clientService, hubContext)
        {
        }

        public async Task OnAddChannel(ChannelSummaryResponse channel)
        {
            var clientIds = await GetNotMutedChannelClientConnectionIdsAsync(channel.Id);
            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.ChannelCreated, channel);
        }

        public async Task OnUpdateChannel(ChannelSummaryResponse channel)
        {
            var clientIds = await GetNotMutedChannelClientConnectionIdsAsync(channel.Id);

            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.ChannelUpdated, channel);
        }

        public async Task OnCloseChannel(ChannelSummaryResponse channel)
        {
            var clientIds = await GetNotMutedChannelClientConnectionIdsAsync(channel.Id);

            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.ChannelClosed, channel);
        }

        public async Task OnJoinChannel(MemberSummary member, ChannelSummaryResponse channel)
        {
            var clientIds = await GetNotMutedChannelClientConnectionIdsAsync(channel.Id);

            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.MemberJoined, member, channel);
        }

        public async Task OnLeaveChannel(MemberSummary member, Guid channelId)
        {
            var clientIds = await GetNotMutedChannelClientConnectionIdsAsync(channelId);
            var senderClients = await MemberService.GetMemberClientsAsync(member.Id);
            clientIds.AddRange(senderClients.Select(x => x.ClientConnectionId));

            // Tell the people in this room that you've leaved
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.MemberLeft, member, channelId);
        }

        public async Task OnDeletedFromChannel(MemberSummary member, Guid channelId)
        {
            var channelClients = await GetNotMutedChannelClientConnectionIdsAsync(channelId);
            var deletingMemberClients = (await MemberService.GetMemberClientsAsync(member.Id)).Select(client => client.ClientConnectionId).ToList();

            await HubContext.Clients.Clients(deletingMemberClients).SendAsync(HubEvents.YouAreDeleted, member, channelId);
            await HubContext.Clients.Clients(channelClients).SendAsync(HubEvents.MemberDeleted, member, channelId);
        }
    }
}