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
            var clientIds = await GetChannelClientConnectionIdsAsync(channel.Id);

            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.ChannelCreated, channel);
        }

        public async Task OnUpdateChannel(ChannelSummaryResponse channel)
        {
            var clientIds = await GetChannelClientConnectionIdsAsync(channel.Id);

            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.ChannelUpdated, channel);
        }

        public async Task OnCloseChannel(Guid channelId)
        {
            var clientIds = await GetChannelClientConnectionIdsAsync(channelId);

            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.ChannelClosed, channelId);
        }

        public async Task OnJoinChannel(ChannelSummaryResponse channel)
        {
            var clientIds = await GetChannelClientConnectionIdsAsync(channel.Id);

            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.MemberJoined, channel);
        }

        public async Task OnJoinChannel(ChannelSummaryResponse channel, Guid memberId)
        {
            var clientIds = await GetChannelMemberClientConnectionIdsAsync(channel.Id, memberId);

            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.MemberJoined, channel);
        }

        public async Task OnLeaveChannel(MemberSummaryResponse member, Guid channelId)
        {
            var senderClientsIds = (await MemberService.GetMemberClientsAsync(member.Id)).Select(client => client.ClientConnectionId).ToList();
 
            await HubContext.Clients.Clients(senderClientsIds).SendAsync(HubEvents.MemberLeft, channelId);
        }

        public async Task OnDeletedFromChannel(MemberSummaryResponse member, Guid channelId)
        {
            var deletingMemberClients = (await MemberService.GetMemberClientsAsync(member.Id)).Select(client => client.ClientConnectionId).ToList();

            await HubContext.Clients.Clients(deletingMemberClients).SendAsync(HubEvents.YouAreDeleted, channelId);
        }
    }
}
