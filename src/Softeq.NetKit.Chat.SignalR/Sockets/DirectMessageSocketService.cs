// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel;
using Softeq.NetKit.Chat.SignalR.Hubs.Notifications;
using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;

namespace Softeq.NetKit.Chat.SignalR.Sockets
{
    public class DirectMessageSocketService : IDirectMessageSocketService
    {
        private readonly IDirectMessageNotificationService _directMessageNotificationService;
        private readonly IDirectChannelService _directChannelService;
        private readonly IMemberService _memberService;

        public DirectMessageSocketService(
            IDirectMessageNotificationService directMessageNotificationService, 
            IDirectChannelService directChannelService, 
            IMemberService memberService)
        {
            Ensure.That(directMessageNotificationService).IsNotNull();
            Ensure.That(directChannelService).IsNotNull();
            Ensure.That(memberService).IsNotNull();

            _directMessageNotificationService = directMessageNotificationService;
            _directChannelService = directChannelService;
            _memberService = memberService;
        }

        public async Task<DirectMessageResponse> AddMessageAsync(CreateDirectMessageRequest request)
        {
            var message = await _directChannelService.AddMessageAsync(request);
            var owner = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);
            var memberId = await GetInterlocutorIdAsync(request.DirectChannelId, owner.Id);

            await _directMessageNotificationService.OnAddMessage(message, memberId);

            return message;
        }

        public async Task<DirectMessageResponse> UpdateMessageAsync(UpdateDirectMessageRequest request)
        {
            var updatedMessage = await _directChannelService.UpdateMessageAsync(request);
            var owner = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);
            var memberId = await GetInterlocutorIdAsync(request.DirectChannelId, owner.Id);

            await _directMessageNotificationService.OnUpdateMessage(updatedMessage, memberId);

            return updatedMessage;
        }

        public async Task<DirectMessageResponse> DeleteMessageAsync(string saasUserId, Guid messageId, Guid directChannelId)
        {
            var message = await _directChannelService.ArchiveMessageAsync(messageId, saasUserId);
            var owner = await _memberService.GetMemberBySaasUserIdAsync(saasUserId);
            var memberId = await GetInterlocutorIdAsync(directChannelId, owner.Id);

            await _directMessageNotificationService.OnDeleteMessage(message, memberId);

            return message;
        }

        private async Task<Guid> GetInterlocutorIdAsync(Guid channelId, Guid ownerId)
        {
            var directChannelResponse = await _directChannelService.GetDirectChannelByIdAsync(channelId);

            return directChannelResponse.Owner.Id == ownerId ? directChannelResponse.Member.Id : directChannelResponse.Owner.Id;
        }
    }
}
