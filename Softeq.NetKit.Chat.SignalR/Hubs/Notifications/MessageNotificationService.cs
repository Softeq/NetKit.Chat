// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.SignalR;
using Softeq.NetKit.Chat.Domain.Services;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public class MessageNotificationService : BaseNotificationService, IMessageNotificationService
    {
        private readonly IChannelService _channelService;

        public MessageNotificationService(
            IChannelMemberService channelMemberService,
            IMemberService memberService,
            IHubContext<ChatHub> hubContext,
            IChannelService channelService)
            : base(channelMemberService, memberService, hubContext)
        {
            Ensure.That(channelService).IsNotNull();

            _channelService = channelService;
        }

        public async Task OnAddMessage(MemberSummary member, MessageResponse message, string clientConnectionId)
        {
            var callerRequest = new ChannelRequest(member.SaasUserId, message.ChannelId);

            var clientIds = await GetChannelClientsExceptCallerAsync(callerRequest, clientConnectionId);

            // Notify all clients for the uploaded message
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.MessageAdded, message);
        }

        public async Task OnDeleteMessage(MemberSummary member, MessageResponse message)
        {
            var channelSummary = await _channelService.GetChannelSummaryAsync(new ChannelRequest(member.SaasUserId, message.ChannelId));

            var clientIds = await GetChannelClientsAsync(new ChannelRequest(member.SaasUserId, channelSummary.Id));

            // Notify all clients for the deleted message
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.MessageDeleted, message.Id, channelSummary);
        }

        public async Task OnUpdateMessage(MemberSummary member, MessageResponse message)
        {
            var clientIds = await GetChannelClientsAsync(new ChannelRequest(member.SaasUserId, message.ChannelId));

            // Notify all clients for the deleted message
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.MessageUpdated, message);
        }

        public async Task OnAddMessageAttachment(MemberSummary member, MessageResponse message)
        {
            var channel = await _channelService.GetChannelByIdAsync(message.ChannelId);

            var clientIds = await GetChannelClientsAsync(new ChannelRequest(member.SaasUserId, channel.Id));

            // Notify all clients for the uploaded message
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.AttachmentAdded, channel.Name);
        }

        public async Task OnDeleteMessageAttachment(MemberSummary member, MessageResponse message)
        {
            var channel = await _channelService.GetChannelByIdAsync(message.ChannelId);

            var clientIds = await GetChannelClientsAsync(new ChannelRequest(member.SaasUserId, channel.Id));

            // Notify all clients for the uploaded message
            await HubContext.Clients.Clients(clientIds).SendAsync(HubEvents.AttachmentDeleted, channel.Name);
        }

        public async Task OnChangeLastReadMessage(List<MemberSummary> members, MessageResponse message)
        {
            var channel = await _channelService.GetChannelByIdAsync(message.ChannelId);

            var channelRequest = new ChannelRequest(members.First().SaasUserId, message.ChannelId);
            var notifyMemberIds = members.Select(x => x.Id).ToList();
            var connectionIds = await GetNotMutedChannelMembersConnectionsAsync(channelRequest, notifyMemberIds);

            // Notify owner about read message
            await HubContext.Clients.Clients(connectionIds).SendAsync(HubEvents.LastReadMessageChanged, channel.Name);
        }
    }
}