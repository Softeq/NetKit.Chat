// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.SignalR;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public abstract class BaseNotificationService
    {
        private readonly IClientService _clientService;
        private readonly IChannelMemberService _channelMemberService;

        protected BaseNotificationService(IChannelMemberService channelMemberService, IMemberService memberService, IClientService clientService, IHubContext<ChatHub> hubContext)
        {
            Ensure.That(channelMemberService).IsNotNull();
            Ensure.That(memberService).IsNotNull();
            Ensure.That(hubContext).IsNotNull();
            Ensure.That(clientService).IsNotNull();
            
            MemberService = memberService;
            HubContext = hubContext;
            _channelMemberService = channelMemberService;
            _clientService = clientService;
        }

        protected IHubContext<ChatHub> HubContext { get; }

        protected IMemberService MemberService { get; }

        protected async Task<List<string>> GetNotMutedChannelClientConnectionIdsAsync(Guid channelId)
        {
            return (await _clientService.GetNotMutedChannelClientConnectionIdsAsync(channelId)).ToList();
        }

        protected async Task<List<string>> GetChannelClientConnectionIdsAsync(Guid channelId)
        {
            return (await _clientService.GetChannelClientConnectionIdsAsync(channelId)).ToList();
        }

        protected async Task<List<string>> GetChannelMemberClientConnectionIdsAsync(Guid channelId, Guid memberId)
        {
            return (await _clientService.GetChannelMemberClientConnectionIdsAsync(channelId, memberId)).ToList();
        }

        protected async Task<List<string>> GetNotMutedChannelMembersConnectionsAsync(Guid channelId, IReadOnlyCollection<Guid> notifyMemberIds)
        {
            var channelMembers = await _channelMemberService.GetChannelMembersAsync(channelId);

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
