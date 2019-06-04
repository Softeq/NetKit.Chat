// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Microsoft.AspNetCore.SignalR;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public class MessageNotificationService : BaseNotificationService, IMessageNotificationService
    {
        private readonly IChannelService _channelService;

        public MessageNotificationService(
            IChannelMemberService channelMemberService,
            IMemberService memberService,
            IClientService clientService,
            IHubContext<ChatHub> hubContext,
            IChannelService channelService)
            : base(channelMemberService, memberService, clientService, hubContext)
        {
            Ensure.That(channelService).IsNotNull();
            _channelService = channelService;
        }

        public async Task OnAddMessage(MessageResponse message, RecipientType recipientType, string callerConnectionId = null)
        {
            var clientIds = await GetChannelClientConnectionIdsAsync(message.ChannelId);
            var exceptList = await GetExceptConnectionIdsListAsync(recipientType, message.ChannelId, message.Sender.Id, callerConnectionId ?? string.Empty);

            var clientIdsExceptCaller = clientIds.Except(exceptList).ToList();

            await HubContext.Clients.Clients(clientIdsExceptCaller).SendAsync(HubEvents.MessageAdded, message);
        }

        public async Task OnDeleteMessage(MessageResponse message)
        {
            var clientIds = await GetChannelClientConnectionIdsAsync(message.ChannelId);

            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.MessageDeleted, message.Id);
        }

        public async Task OnUpdateMessage(MessageResponse message)
        {
            var clientIds = await GetChannelClientConnectionIdsAsync(message.ChannelId);

            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.MessageUpdated, message);
        }

        public async Task OnAddMessageAttachment(Guid channelId)
        {
            var channel = await _channelService.GetChannelByIdAsync(channelId);

            var clientIds = await GetChannelClientConnectionIdsAsync(channelId);

            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.AttachmentAdded, channel.Id);
        }

        public async Task OnDeleteMessageAttachment(MessageResponse message)
        {
            var channel = await _channelService.GetChannelByIdAsync(message.ChannelId);

            var clientIds = await GetChannelClientConnectionIdsAsync(channel.Id);

            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.AttachmentDeleted, channel.Id);
        }

        public async Task OnChangeLastReadMessage(List<Guid> notifyMemberIds, Guid channelId)
        {
            var channel = await _channelService.GetChannelByIdAsync(channelId);

            var connectionIds = await GetNotMutedChannelMembersConnectionsAsync(channelId, notifyMemberIds);

            await HubContext.Clients.Clients(connectionIds).SendAsync(HubEvents.LastReadMessageChanged, channel.Id);
        }

        private async Task<IEnumerable<string>> GetExceptConnectionIdsListAsync(RecipientType recipientType, Guid channelId, Guid memberId, string callerConnectionId)
        {
            var exceptList = Enumerable.Empty<string>().ToList();

            if (recipientType == RecipientType.AllChannelConnections)
            {
                exceptList = await GetChannelClientConnectionIdsAsync(channelId);
            }
            else if (recipientType == RecipientType.AllExceptMemberConnections)
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