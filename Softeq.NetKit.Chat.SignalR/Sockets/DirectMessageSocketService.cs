// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectMembers;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers;
using Softeq.NetKit.Chat.SignalR.Hubs.Notifications;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.SignalR.Sockets
{
    public class DirectMessageSocketService : IDirectMessageSocketService
    {
        private readonly IDirectMessageService _directMessageService;
        private readonly IDirectMessageNotificationService _directMessageNotificationService;

        public DirectMessageSocketService(IDirectMessageService directMessageService, IDirectMessageNotificationService directMessageNotificationService)
        {
            _directMessageService = directMessageService;
            _directMessageNotificationService = directMessageNotificationService;
        }

        public async Task<DirectChannelResponse> CreateDirectMembers(CreateDirectChannelRequest request, string connectionId)
        {
            var createDirectMembersResponse = await _directMessageService.CreateDirectChannel(request);

            await _directMessageNotificationService.OnCreateDirectMembers(createDirectMembersResponse, connectionId);

            return createDirectMembersResponse;
        }
    }
}
