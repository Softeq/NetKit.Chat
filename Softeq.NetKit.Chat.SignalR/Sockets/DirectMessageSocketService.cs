// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectMembers;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.SignalR.Hubs.Notifications;

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

        public async Task<CreateDirectMembersResponse> CreateDirectMembers(CreateDirectMembersRequest request)
        {
            var createDirectMembersResponse = await _directMessageService.CreateDirectMembers(request);

            await _directMessageNotificationService.OnCreateDirectMembers(createDirectMembersResponse);

            return createDirectMembersResponse;
        }
    }
}
