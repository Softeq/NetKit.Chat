// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.AspNetCore.SignalR;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public class DirectMessageNotificationService : BaseNotificationService, IDirectMessageNotificationService
    {
        public DirectMessageNotificationService(
            IChannelMemberService channelMemberService,
            IMemberService memberService,
            IClientService clientService,
            IHubContext<ChatHub> hubContext)
            : base(channelMemberService, memberService, clientService, hubContext)
        { }

        public async Task OnCreateDirectMembers(CreateDirectMembersResponse request, string connectionId)
        {
            var clientIds = new List<string>
            {
                request.Owner.Id.ToString(),
                request.Member.Id.ToString()
            };

            // Tell the people that direct was created.
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.DirectMembersCreated, request.DirectMembersId, request.Owner.Id, connectionId);
        }
    }
}
