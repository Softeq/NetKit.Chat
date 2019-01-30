// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public class DirectMessageNotificationService : BaseNotificationService, IDirectMessageNotificationService
    {
        public DirectMessageNotificationService(IChannelMemberService channelMemberService, IMemberService memberService, IClientService clientService, IHubContext<ChatHub> hubContext) : base(channelMemberService, memberService, clientService, hubContext)
        {
        }

        public Task OnAddMessage()
        {
            // TODO
            throw new System.NotImplementedException();
        }

        public Task OnDeleteMessage()
        {
            // TODO
            throw new System.NotImplementedException();
        }

        public Task OnUpdateMessage()
        {
            // TODO
            throw new System.NotImplementedException();
        }
    }
}
