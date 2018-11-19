// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Softeq.NetKit.Chat.Domain.Channel;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Domain.Services.Exceptions;
using Softeq.NetKit.Chat.Infrastructure.SignalR.Hubs;
using Softeq.NetKit.Chat.Infrastructure.SignalR.Resources;
using Softeq.Serilog.Extension;

namespace Softeq.NetKit.Chat.Infrastructure.SignalR.Sockets
{
    internal class ChannelSocketService : IChannelSocketService
    {
        private readonly IChannelService _channelService;
        private readonly IMemberService _memberService;
        private readonly IChannelNotificationHub _channelNotificationHub;
        private readonly ILogger _logger;

        public ChannelSocketService(
            IChannelService channelService,
            ILogger logger,
            IMemberService memberService,
            IChannelNotificationHub channelNotificationHub)
        {
            _channelNotificationHub = channelNotificationHub;
            _channelService = channelService;
            _logger = logger;
            _memberService = memberService;
        }

        public async Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest createChannelRequest)
        {
            if (string.IsNullOrEmpty(createChannelRequest.Name))
            {
                throw new Exception(LanguageResources.RoomRequired);
            }

            try
            {
                var channel = await _channelService.CreateChannelAsync(createChannelRequest);
                var user = await _memberService.GetMemberSummaryBySaasUserIdAsync(createChannelRequest.SaasUserId);

                await _channelNotificationHub.OnAddChannel(user, channel, createChannelRequest.ClientConnectionId);
                //todo filter creator connection id on join channel
                await _channelNotificationHub.OnJoinChannel(user, channel);

                return channel;
            }
            catch (NotFoundException ex)
            {
                _logger.Event("ChannelDoesNotExist").With.Message("{@ChannelName}", createChannelRequest.Name).Exception(ex).AsError();
                throw new Exception(string.Format(LanguageResources.RoomNotFound, createChannelRequest.Name));
            }
        }

        public async Task<ChannelSummaryResponse> UpdateChannelAsync(UpdateChannelRequest request)
        {
            if (String.IsNullOrEmpty(request.Name))
            {
                throw new Exception(LanguageResources.RoomRequired);
            }

            if (request.Name.Contains(' '))
            {
                throw new Exception(LanguageResources.RoomInvalidNameSpaces);
            }

            try
            {
                var channel = await _channelService.GetChannelByIdAsync(new ChannelRequest(request.SaasUserId, request.ChannelId));

                var member = await _memberService.GetMemberSummaryBySaasUserIdAsync(request.SaasUserId);

                if (channel.CreatorId != member.Id)
                {
                    throw new Exception(string.Format(LanguageResources.RoomAccessPermission, channel.Name));
                }

                await _channelService.UpdateChannelAsync(request);

                var channelSummary = await _channelService.GetChannelSummaryAsync(new ChannelRequest(request.SaasUserId, request.ChannelId));

                await _channelNotificationHub.OnUpdateChannel(member, channelSummary);

                return channelSummary;
            }
            catch (NotFoundException ex)
            {
                _logger.Event("ChannelDoesNotExist").With.Message("{@ChannelName}", request.Name).Exception(ex).AsError();
                throw new Exception(string.Format(LanguageResources.RoomNotFound, request.Name));
            }
        }

        public async Task CloseChannelAsync(ChannelRequest request)
        {
            try
            {
                var channel = await _channelService.GetChannelByIdAsync(request);
                var member = await _memberService.GetMemberSummaryBySaasUserIdAsync(request.SaasUserId);

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

                await _channelNotificationHub.OnCloseChannel(member, channelSummary);
                await _channelNotificationHub.OnUpdateChannel(member, channelSummary);
            }
            catch (ServiceException ex)
            {
                _logger.Event("ChannelDoesNotExist").With.Message("{@ChannelId}", request.ChannelId).Exception(ex).AsError();
                throw new Exception(string.Format(LanguageResources.RoomNotFound, request.ChannelId));
            }
        }

        public async Task JoinToChannelAsync(JoinToChannelRequest request)
        {
            if (request.ChannelId == Guid.Empty)
            {
                throw new Exception(LanguageResources.Join_RoomRequired);
            }

            // Locate the room, does NOT have to be open
            try
            {
                var member = await _memberService.GetMemberSummaryBySaasUserIdAsync(request.SaasUserId);
                var isMemberExistInChannel = await _channelService.CheckIfMemberExistInChannelAsync(new InviteMemberRequest(request.SaasUserId, request.ChannelId, member.Id));
                if (!isMemberExistInChannel)
                {
                    await _channelService.JoinToChannelAsync(request);

                    var channel = await _channelService.GetChannelSummaryAsync(new ChannelRequest(request.SaasUserId, request.ChannelId));
   
                    await _channelNotificationHub.OnJoinChannel(member, channel);
                }
            }
            catch (NotFoundException ex)
            {
                _logger.Event("ChannelDoesNotExist").With.Message("{@ChannelId}", request.ChannelId).Exception(ex).AsError();
                throw new Exception(string.Format(LanguageResources.RoomNotFound, request.ChannelId));
            }
        }

        public async Task LeaveChannelAsync(ChannelRequest request)
        {
            try
            {
                var member = await _memberService.GetMemberSummaryBySaasUserIdAsync(request.SaasUserId);
                var channel = await _channelService.GetChannelSummaryAsync(request);

                var isMemberExistInChannel = await _channelService.CheckIfMemberExistInChannelAsync(new InviteMemberRequest(request.SaasUserId, request.ChannelId, member.Id));

                if (!isMemberExistInChannel)
                {
                    throw new Exception(string.Format(LanguageResources.UserNotInRoom, member.UserName, channel.Name));
                }

                await _channelService.LeaveChannelAsync(request);

                await _channelNotificationHub.OnLeaveChannel(member, channel);
            }
            catch (NotFoundException ex)
            {
                _logger.Event("ChannelDoesNotExist").With.Message("{@ChannelId}", request.ChannelId).Exception(ex).AsError();
                throw new Exception(string.Format(LanguageResources.RoomNotFound, request.ChannelId));
            }
        }

        public async Task InviteMemberAsync(InviteMemberRequest request)
        {
            if (request.MemberId == Guid.Empty)
            {
                throw new Exception(LanguageResources.Invite_UserRequired);
            }

            try
            {
                var member = await _memberService.GetMemberSummaryBySaasUserIdAsync(request.SaasUserId);
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

                await _memberService.InviteMemberAsync(request);

                await _channelNotificationHub.OnJoinChannel(invitedMember, channel);
            }
            catch (ServiceException ex)
            {
                if (ex.Errors.Any(x => x.Description == "Member does not exist."))
                {
                    _logger.Event("MemberDoesNotExist").With.Message("{@MemberId}", request.MemberId).Exception(ex).AsError();
                    throw new Exception(string.Format(LanguageResources.UserNotFound, request.MemberId));
                }
                if (ex.Errors.Any(x => x.Description == "Channel does not exist."))
                {
                    _logger.Event("ChannelDoesNotExist").With.Message("{@ChannelId}", request.ChannelId).Exception(ex).AsError();
                    throw new Exception(string.Format(LanguageResources.RoomNotFound, request.ChannelId));
                }
            }
        }

        public async Task InviteMembersAsync(InviteMembersRequest request)
        {
            foreach (var invitedMember in request.InvitedMembers)
            {
                var member = await _memberService.GetMemberSummaryBySaasUserIdAsync(invitedMember);
                var inviteMemberRequest = new InviteMemberRequest(request.SaasUserId, request.ChannelId, member.Id);
                await InviteMemberAsync(inviteMemberRequest);
            }
        }

        public async Task MuteChannelAsync(ChannelRequest request)
        {
            try
            {
                var member = await _memberService.GetMemberSummaryBySaasUserIdAsync(request.SaasUserId);
                var channel = await _channelService.GetChannelByIdAsync(request);

                var isMemberExistInChannel = await _channelService.CheckIfMemberExistInChannelAsync(new InviteMemberRequest(request.SaasUserId, request.ChannelId, member.Id));

                if (!isMemberExistInChannel)
                {
                    throw new Exception(string.Format(LanguageResources.UserNotInRoom, member.UserName, channel.Name));
                }

                await _channelService.MuteChannelAsync(request);
            }
            catch (NotFoundException ex)
            {
                _logger.Event("ChannelDoesNotExist").With.Message("{@ChannelId}", request.ChannelId).Exception(ex).AsError();
                throw new Exception(string.Format(LanguageResources.RoomNotFound, request.ChannelId));
            }
        }
    }
}