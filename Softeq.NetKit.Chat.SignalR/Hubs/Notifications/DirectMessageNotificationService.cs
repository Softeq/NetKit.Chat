// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public class DirectMessageNotificationService : BaseNotificationService, IDirectMessageNotificationService
    {
        public DirectMessageNotificationService(IChannelMemberService channelMemberService, IMemberService memberService, IClientService clientService, IHubContext<ChatHub> hubContext) 
            : base(channelMemberService, memberService, clientService, hubContext)
        { }

        public async Task OnAddMessage(DirectMessageResponse message, Guid memberId)
        {
            await HubContext.Clients.User(memberId.ToString()).SendAsync(HubEvents.DirectMessageAdded, message);
        }

        public async Task OnDeleteMessage(DirectMessageResponse message, Guid memberId)
        {
            await HubContext.Clients.User(memberId.ToString()).SendAsync(HubEvents.DirectMessageDeleted, message); ;
        }

        public async Task OnUpdateMessage(DirectMessageResponse message, Guid memberId)
        {
            await HubContext.Clients.User(memberId.ToString()).SendAsync(HubEvents.DirectMessageUpdated, message);
        }
    }
}
