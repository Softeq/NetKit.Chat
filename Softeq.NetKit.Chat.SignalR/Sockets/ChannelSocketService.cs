// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Resources;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.SignalR.Hubs.Notifications;

namespace Softeq.NetKit.Chat.SignalR.Sockets
{
    internal class ChannelSocketService : IChannelSocketService
    {
        private readonly IChannelService _channelService;
        private readonly IMemberService _memberService;
        private readonly IChannelNotificationService _channelNotificationService;

        public ChannelSocketService(
            IChannelService channelService,
            IMemberService memberService,
            IChannelNotificationService channelNotificationService)
        {
            _channelNotificationService = channelNotificationService;
            _channelService = channelService;
            _memberService = memberService;
        }

        public async Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest createChannelRequest)
        {
            if (string.IsNullOrEmpty(createChannelRequest.Name))
            {
                throw new Exception(LanguageResources.RoomRequired);
            }

            var channel = await _channelService.CreateChannelAsync(createChannelRequest);
            var user = await _memberService.GetMemberBySaasUserIdAsync(createChannelRequest.SaasUserId);

            await _channelNotificationService.OnAddChannel(user, channel, createChannelRequest.ClientConnectionId);
            //todo filter creator connection id on join channel
            await _channelNotificationService.OnJoinChannel(user, channel);

            return channel;
        }

        public async Task<ChannelSummaryResponse> UpdateChannelAsync(UpdateChannelRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new Exception(LanguageResources.RoomRequired);
            }

            if (request.Name.Contains(' '))
            {
                throw new Exception(LanguageResources.RoomInvalidNameSpaces);
            }

            var channel = await _channelService.GetChannelByIdAsync(request.ChannelId);

            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);

            if (channel.CreatorId != member.Id)
            {
                throw new Exception(string.Format(LanguageResources.RoomAccessPermission, channel.Name));
            }

            await _channelService.UpdateChannelAsync(request);

            var channelSummary = await _channelService.GetChannelSummaryAsync(new ChannelRequest(request.SaasUserId, request.ChannelId));

            await _channelNotificationService.OnUpdateChannel(member, channelSummary);

            return channelSummary;
        }

        public async Task CloseChannelAsync(ChannelRequest request)
        {
            var channel = await _channelService.GetChannelByIdAsync(request.ChannelId);
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);

            if (channel.CreatorId != member.Id && member.Role != UserRole.Admin)
            {
                throw new Exception(string.Format(LanguageResources.RoomOwnerRequired, channel.Name));
            }
            if (channel.IsClosed)
            {
                throw new Exception(string.Format(LanguageResources.RoomAlreadyClosed, channel.Name));
            }

            await _channelService.CloseChannelAsync(request);

            var channelSummary = await _channelService.GetChannelSummaryAsync(request);

            await _channelNotificationService.OnCloseChannel(member, channelSummary);
            await _channelNotificationService.OnUpdateChannel(member, channelSummary);
        }

        public async Task JoinToChannelAsync(JoinToChannelRequest request)
        {
            if (request.ChannelId == Guid.Empty)
            {
                throw new Exception(LanguageResources.Join_RoomRequired);
            }

            // Locate the room, does NOT have to be open
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);
            var isMemberExistInChannel = await _channelService.CheckIfMemberExistInChannelAsync(new InviteMemberRequest(request.SaasUserId, request.ChannelId, member.Id));
            if (!isMemberExistInChannel)
            {
                await _channelService.JoinToChannelAsync(request);
                var channel = await _channelService.GetChannelSummaryAsync(new ChannelRequest(request.SaasUserId, request.ChannelId));
                await _channelNotificationService.OnJoinChannel(member, channel);
            }
        }

        public async Task LeaveChannelAsync(ChannelRequest request)
        {
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);

            await _channelService.RemoveMemberFromChannelAsync(request);
            await _channelNotificationService.OnLeaveChannel(member, request.ChannelId);
        }

        public async Task<ChannelResponse> InviteMemberAsync(InviteMemberRequest request)
        {
            if (request.MemberId == Guid.Empty)
            {
                throw new Exception(LanguageResources.Invite_UserRequired);
            }

            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);
            var invitedMember = await _memberService.GetMemberByIdAsync(request.MemberId);
            if (member.Id == invitedMember.Id)
            {
                throw new Exception(LanguageResources.Invite_CannotInviteSelf);
            }

            var channel = await _channelService.GetChannelSummaryAsync(new ChannelRequest(request.SaasUserId, request.ChannelId));
            if (channel.IsClosed)
            {
                throw new Exception(string.Format(LanguageResources.RoomClosed, channel.Name));
            }

            var inviteMemberResponse = await _memberService.InviteMemberAsync(request);

            await _channelNotificationService.OnJoinChannel(invitedMember, channel);

            return inviteMemberResponse;
        }

        public async Task<ChannelResponse> InviteMultipleMembersAsync(InviteMembersRequest request)
        {
            var response = default(ChannelResponse);

            foreach (var invitedMember in request.InvitedMembers)
            {
                var member = await _memberService.GetMemberBySaasUserIdAsync(invitedMember);
                var inviteMemberRequest = new InviteMemberRequest(request.SaasUserId, request.ChannelId, member.Id);
                response = await InviteMemberAsync(inviteMemberRequest);
            }

            return response;
        }

        public async Task DeleteMemberAsync(DeleteMemberRequest request)
        {
            if (request.MemberId == Guid.Empty)
            {
                //TODO apply validation
                throw new Exception(LanguageResources.RemoveAdmin_UserRequired);
            }

            var memberToDelete = await _memberService.GetMemberByIdAsync(request.MemberId);
            if (memberToDelete.SaasUserId == request.SaasUserId)
            {
                throw new Exception($"Can not delete yourself. Use {nameof(LeaveChannelAsync)} instead.");
            }

            var channel = await _channelService.GetChannelByIdAsync(request.ChannelId);
            var currentMember = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);

            if (channel.CreatorId != currentMember.Id && currentMember.Role != UserRole.Admin)
            {
                //TODO apply validation
                throw new Exception(LanguageResources.AdminRequired);
            }

            await _channelService.RemoveMemberFromChannelAsync(new ChannelRequest(memberToDelete.SaasUserId, request.ChannelId));
            await _channelNotificationService.OnDeletedFromChannel(memberToDelete, channel.Id, request.ClientConnectionId);
        }

        public async Task MuteChannelAsync(ChannelRequest request)
        {
            var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);
            var channel = await _channelService.GetChannelByIdAsync(request.ChannelId);

            var isMemberExistInChannel = await _channelService.CheckIfMemberExistInChannelAsync(new InviteMemberRequest(request.SaasUserId, request.ChannelId, member.Id));
            if (!isMemberExistInChannel)
            {
                throw new Exception(string.Format(LanguageResources.UserNotInRoom, member.UserName, channel.Name));
            }

            await _channelService.MuteChannelAsync(request);
        }

        public async Task PinChannelAsync(ChannelRequest request)
        {
            try
            {
                var member = await _memberService.GetMemberBySaasUserIdAsync(request.SaasUserId);
                var channel = await _channelService.GetChannelByIdAsync(request.ChannelId);

                var isMemberExistInChannel = await _channelService.CheckIfMemberExistInChannelAsync(new InviteMemberRequest(request.SaasUserId, request.ChannelId, member.Id));

                if (!isMemberExistInChannel)
                {
                    throw new Exception(String.Format(LanguageResources.UserNotInRoom, member.UserName, channel.Name));
                }

                await _channelService.PinChannelAsync(request);
            }
            catch (NotFoundException ex)
            {
                _logger.Event("ChannelNotFound").With.Message("Pinned channel does not exist. ChannelId: {channelId}", request.ChannelId).Exception(ex).AsError();
                throw new Exception(String.Format(LanguageResources.RoomNotFound, request.ChannelId));
            }
        }
    }
}