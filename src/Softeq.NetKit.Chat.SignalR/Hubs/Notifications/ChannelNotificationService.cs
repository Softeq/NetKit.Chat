// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Member;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;

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

        public async Task OnJoinDirectChannel(MemberSummaryResponse member, ChannelSummaryResponse channel)
        {
            var clientIds = await GetChannelMemberClientConnectionIdsAsync(channel.Id, member.Id);

            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.MemberJoined, member, channel);
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
