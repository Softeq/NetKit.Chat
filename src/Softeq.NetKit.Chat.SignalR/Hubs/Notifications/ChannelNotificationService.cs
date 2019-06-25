// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Member;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public class ChannelNotificationService : BaseNotificationService, IChannelNotificationService
    {
        public ChannelNotificationService(IChannelMemberService channelMemberService, IMemberService memberService, IClientService clientService, IHubContext<ChatHub> hubContext)
            : base(channelMemberService, memberService, clientService, hubContext)
        {
        }

        public async Task OnUpdateChannel(ChannelSummaryResponse channel)
        {
            var clientIds = await GetChannelClientConnectionIdsAsync(channel.Id);

            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.ChannelUpdated, channel);
        }

        public async Task OnUpdateChannelPersonalized(ChannelSummaryResponse channel, Guid memberId, string currentConnectionId = "")
        {
            var clientIds = await GetChannelMemberClientConnectionIdsAsync(channel.Id, memberId);

            clientIds = clientIds.Except(new List<string> { currentConnectionId }).ToList();

            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.ChannelUpdated, channel);
        }

        public async Task OnCloseChannel(Guid channelId)
        {
            var clientIds = await GetChannelClientConnectionIdsAsync(channelId);

            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.ChannelClosed, channelId);
        }

        public async Task OnJoinChannel(ChannelSummaryResponse channel, Guid memberId)
        {
            var clientIds = await GetChannelClientConnectionIdsAsync(channel.Id);
            var exceptList = await GetExceptConnectionIdsListAsync(RecipientType.AllExceptMemberConnections, channel.Id, memberId, string.Empty);

            var clientIdsExceptCaller = clientIds.Except(exceptList).ToList();

            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIdsExceptCaller).SendAsync(HubEvents.MemberJoined, channel);
        }

        public async Task OnJoinChannelPersonalized(ChannelSummaryResponse channel, Guid memberId, string currentConnectionId = "")
        {
            var clientIds = await GetChannelMemberClientConnectionIdsAsync(channel.Id, memberId);

            clientIds = clientIds.Except(new List<string> { currentConnectionId }).ToList();

            // Tell the people in this room that you've joined
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.MemberJoined, channel);
        }

        public async Task OnLeaveChannel(MemberSummaryResponse member, Guid channelId)
        {
            var senderClientsIds = (await MemberService.GetMemberClientsAsync(member.Id)).Select(client => client.ClientConnectionId).ToList();
 
            await HubContext.Clients.Clients(senderClientsIds).SendAsync(HubEvents.MemberLeft, channelId);
        }

        private async Task<IEnumerable<string>> GetExceptConnectionIdsListAsync(RecipientType recipientType, Guid channelId, Guid memberId, string callerConnectionId)
        {
            var exceptList = Enumerable.Empty<string>().ToList();

            if (recipientType == RecipientType.AllExceptMemberConnections)
            {
                var memberClientIds = await GetChannelMemberClientConnectionIdsAsync(channelId, memberId);
                exceptList.AddRange(memberClientIds);
            }
            else if (recipientType == RecipientType.AllExceptCallerConnectionId)
            {
                exceptList.Add(callerConnectionId);
            }

            return exceptList;
        }
    }
}
