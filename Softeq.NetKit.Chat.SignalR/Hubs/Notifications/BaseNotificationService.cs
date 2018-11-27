// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.SignalR;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.ChannelMember;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public abstract class BaseNotificationService
    {
        protected BaseNotificationService(IChannelMemberService channelMemberService, IMemberService memberService, IHubContext<ChatHub> hubContext)
        {
            Ensure.That(channelMemberService).IsNotNull();
            Ensure.That(memberService).IsNotNull();
            Ensure.That(hubContext).IsNotNull();

            ChannelMemberService = channelMemberService;
            MemberService = memberService;
            HubContext = hubContext;
        }

        protected IHubContext<ChatHub> HubContext { get; }

        protected IChannelMemberService ChannelMemberService { get; }

        protected IMemberService MemberService { get; }

        protected async Task<List<string>> GetChannelClientsAsync(ChannelRequest request)
        {
            // TODO: Change this code. Recommended to use Clients.Group()
            var members = await ChannelMemberService.GetChannelMembersAsync(request.ChannelId);

            return await FilterClients(members, request.ClientConnectionId);
        }

        protected async Task<List<string>> GetChannelClientsExceptCallerAsync(ChannelRequest request, string callerConnectionId)
        {
            // TODO: Change this code. Recommended to use Clients.Group()
            var members = await ChannelMemberService.GetChannelMembersAsync(request.ChannelId);

            var mutedMemberIds = members.Where(x => x.IsMuted)
                .Select(x => x.MemberId)
                .ToList();

            var mutedConnectionClientIds = (await MemberService.GetClientsByMemberIds(mutedMemberIds))
                .Select(x => x.ConnectionClientId)
                .ToList();
            mutedConnectionClientIds.Add(callerConnectionId);

            var clients = new List<string>();
            foreach (var item in members)
            {
                var memberClients = (await MemberService.GetMemberClientsAsync(item.MemberId))
                    .Select(x => x.ClientConnectionId)
                    .Except(mutedConnectionClientIds)
                    .ToList();

                clients.AddRange(memberClients);
            }

            // TODO: clear connectionIds in database. There are about 4000 connections for only 3 users at the moment
            return clients;
        }

        protected async Task<List<string>> FilterClients(IReadOnlyCollection<ChannelMemberResponse> members, string clientConnectionId)
        {
            var mutedMemberIds = members.Where(x => x.IsMuted)
                .Select(x => x.MemberId)
                .ToList();

            var mutedConnectionClientIds = (await MemberService.GetClientsByMemberIds(mutedMemberIds))
                .Select(x => x.ConnectionClientId)
                .ToList();

            var clients = new List<string>();
            foreach (var item in members)
            {
                var memberClients = (await MemberService.GetMemberClientsAsync(item.MemberId))
                    .Where(x => x.ClientConnectionId != clientConnectionId)
                    .Select(x => x.ClientConnectionId)
                    .Except(mutedConnectionClientIds)
                    .ToList();

                clients.AddRange(memberClients);
            }

            return clients;
        }

        protected async Task<List<string>> GetNotMutedChannelMembersConnectionsAsync(Guid channelId, IReadOnlyCollection<Guid> notifyMemberIds)
        {
            var channelMembers = await ChannelMemberService.GetChannelMembersAsync(channelId);

            var notMutedMemberIds = channelMembers.Where(x => !x.IsMuted && notifyMemberIds.Contains(x.MemberId))
                .Select(x => x.MemberId)
                .ToList();

            var notMutedConnectionClientIds = (await MemberService.GetClientsByMemberIds(notMutedMemberIds))
                .Select(x => x.ConnectionClientId)
                .ToList();

            return notMutedConnectionClientIds;
        }
    }
}