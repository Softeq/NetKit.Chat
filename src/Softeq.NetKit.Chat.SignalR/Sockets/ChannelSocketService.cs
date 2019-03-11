// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Notifications;
using Softeq.NetKit.Chat.Notifications.Services;
using Softeq.NetKit.Chat.SignalR.Hubs.Notifications;
using ChannelRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel.ChannelRequest;
using CreateChannelRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel.CreateChannelRequest;
using UpdateChannelRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel.UpdateChannelRequest;
using MuteChannelRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel.MuteChannelRequest;

namespace Softeq.NetKit.Chat.SignalR.Sockets
{
    internal class ChannelSocketService : IChannelSocketService
    {
        private readonly IChannelService _channelService;
        private readonly IMemberService _memberService;
        private readonly IChannelNotificationService _channelNotificationService;
        private readonly IPushNotificationService _pushNotificationService;

        public ChannelSocketService(
            IChannelService channelService, 
            IMemberService memberService, 
            IChannelNotificationService channelNotificationService,
            IPushNotificationService pushNotificationService)
        {
            Ensure.That(channelService).IsNotNull();
            Ensure.That(memberService).IsNotNull();
            Ensure.That(channelNotificationService).IsNotNull();

            _channelNotificationService = channelNotificationService;
            _pushNotificationService = pushNotificationService;
            _channelService = channelService;
            _memberService = memberService;
        }

        public async Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest request)
        {
            var channel = await _channelService.CreateChannelAsync(request);
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);

            // subscribe creator on channel
            await _pushNotificationService.SubscribeUserOnTagAsync(member.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(channel.Id.ToString()));

            if (request.AllowedMembers != null)
            {
                // subscribe invited members
                foreach (var allowedMemberId in request.AllowedMembers)
                {
                    var chatMember = await _memberService.GetMemberByIdAsync(Guid.Parse(allowedMemberId));
                    if (chatMember == null)
                    {
                        throw new NetKitChatNotFoundException($"Specified chat member {allowedMemberId} is not found.");
                    }

                    await _pushNotificationService.SubscribeUserOnTagAsync(chatMember.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(channel.Id.ToString()));
                }
            }

            await _channelNotificationService.OnAddChannel(channel);
            //todo filter creator connection id on join channel
            await _channelNotificationService.OnJoinChannel(member, channel);

            return channel;
        }
        
        public async Task<ChannelSummaryResponse> CreateDirectChannelAsync(CreateDirectChannelRequest request)
        {
            var channel = await _channelService.CreateDirectChannelAsync(request);

            // subscribe creator on channel
            await _pushNotificationService.SubscribeUserOnTagAsync(request.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(channel.Id.ToString()));
            var member = await _memberService.GetMemberByIdAsync(request.MemberId);
            // subscribe member on channel
            await _pushNotificationService.SubscribeUserOnTagAsync(member.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(channel.Id.ToString()));

            await _channelNotificationService.OnAddChannel(channel);
            //todo filter creator connection id on join channel
            await _channelNotificationService.OnJoinChannel(member, channel);

            return channel;
        }

        public async Task<ChannelSummaryResponse> UpdateChannelAsync(UpdateChannelRequest request)
        {
            await _channelService.UpdateChannelAsync(request);

            var channelSummary = await _channelService.GetChannelSummaryAsync(request.SaasUserId, request.ChannelId);

            await _channelNotificationService.OnUpdateChannel(channelSummary);

            return channelSummary;
        }

        public async Task CloseChannelAsync(ChannelRequest request)
        {
            await _channelService.CloseChannelAsync(request.SaasUserId, request.ChannelId);

            var channelSummary = await _channelService.GetChannelSummaryAsync(request.SaasUserId, request.ChannelId);

            await _channelNotificationService.OnCloseChannel(channelSummary);

            var channelMembers = await _memberService.GetChannelMembersAsync(request.ChannelId);

            foreach (var member in channelMembers)
            {
                // unsubscribe user from channel
                await _pushNotificationService.UnsubscribeUserFromTagAsync(member.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(request.ChannelId.ToString()));
            }

            // TODO [az]: do we need this notification?
            await _channelNotificationService.OnUpdateChannel(channelSummary);
        }

        public async Task JoinToChannelAsync(ChannelRequest request)
        {
            // Locate the room, does NOT have to be open
            await _channelService.JoinToChannelAsync(request.SaasUserId, request.ChannelId);

            var channel = await _channelService.GetChannelSummaryAsync(request.SaasUserId, request.ChannelId);

            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);

            await _pushNotificationService.SubscribeUserOnTagAsync(member.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(request.ChannelId.ToString()));

            await _channelNotificationService.OnJoinChannel(member, channel);
        }

        public async Task<ChannelResponse> InviteMemberAsync(InviteMemberRequest request)
        {
            var inviteMemberResponse = await _memberService.InviteMemberAsync(request.MemberId, request.ChannelId);

            var invitedMember = await _memberService.GetMemberByIdAsync(request.MemberId);

            var channel = await _channelService.GetChannelSummaryAsync(request.SaasUserId, request.ChannelId);

            if (invitedMember != null && channel != null)
            {
                await _pushNotificationService.SubscribeUserOnTagAsync(invitedMember.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(channel.Id.ToString()));
            }

            await _channelNotificationService.OnJoinChannel(invitedMember, channel);

            return inviteMemberResponse;
        }

        public async Task<ChannelResponse> InviteMultipleMembersAsync(InviteMultipleMembersRequest request)
        {
            var response = default(ChannelResponse);

            foreach (var memberId in request.InvitedMembersIds)
            {
                var inviteMemberRequest = new InviteMemberRequest(request.SaasUserId, request.ChannelId, memberId);
                response = await InviteMemberAsync(inviteMemberRequest);
            }

            return response;
        }

        public async Task LeaveChannelAsync(ChannelRequest request)
        {
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);

            await _channelService.LeaveFromChannelAsync(request.SaasUserId, request.ChannelId);

            await _pushNotificationService.UnsubscribeUserFromTagAsync(member.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(request.ChannelId.ToString()));

            await _channelNotificationService.OnLeaveChannel(member, request.ChannelId);
        }

        public async Task DeleteMemberFromChannelAsync(DeleteMemberRequest request)
        {
            var memberToDelete = await _memberService.GetMemberByIdAsync(request.MemberId);

            await _channelService.DeleteMemberFromChannelAsync(request.SaasUserId, request.ChannelId, memberToDelete.Id);

            await _pushNotificationService.UnsubscribeUserFromTagAsync(memberToDelete.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(request.ChannelId.ToString()));

            await _channelNotificationService.OnDeletedFromChannel(memberToDelete, request.ChannelId);
        }

        public async Task MuteChannelAsync(MuteChannelRequest request)
        {
            if (request.IsMuted)
            {
                await _pushNotificationService.UnsubscribeUserFromTagAsync(request.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(request.ChannelId.ToString()));
            }
            else
            {
                await _pushNotificationService.SubscribeUserOnTagAsync(request.SaasUserId, PushNotificationsTagTemplates.GetChatChannelTag(request.ChannelId.ToString()));
            }

            await _channelService.MuteChannelAsync(request.SaasUserId, request.ChannelId, request.IsMuted);
        }
    }
}