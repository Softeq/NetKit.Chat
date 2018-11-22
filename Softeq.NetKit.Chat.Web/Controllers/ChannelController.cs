// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Settings;
using Softeq.NetKit.Chat.SignalR.Sockets;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/channel")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    public class ChannelController : BaseApiController
    {
        private readonly IChannelService _channelService;
        private readonly IChannelSocketService _channelSocketService;
        private readonly IMemberService _memberService;

        public ChannelController(ILogger logger, IChannelService channelService, IChannelSocketService channelSocketService, IMemberService memberService)
            : base(logger)
        {
            Ensure.That(channelService).IsNotNull();
            Ensure.That(channelSocketService).IsNotNull();
            Ensure.That(memberService).IsNotNull();

            _channelService = channelService;
            _channelSocketService = channelSocketService;
            _memberService = memberService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ChannelResponse), StatusCodes.Status200OK)]
        [Route("{channelId:guid}")]
        public async Task<IActionResult> GetChannelInfoByIdAsync(Guid channelId)
        {
            var channel = await _channelService.GetChannelByIdAsync(channelId);
            return Ok(channel);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ChannelSummaryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateChannelAsync([FromBody] CreateChannelRequest request)
        {
            request.SaasUserId = GetCurrentSaasUserId();
            var channel = await _channelSocketService.CreateChannelAsync(request);
            return Ok(channel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ChannelSummaryResponse), StatusCodes.Status200OK)]
        [Route("{channelId:guid}")]
        public async Task<IActionResult> UpdateChannelAsync(Guid channelId, [FromBody] UpdateChannelRequest request)
        {
            request.ChannelId = channelId;
            request.SaasUserId = GetCurrentSaasUserId();
            var channel = await _channelSocketService.UpdateChannelAsync(request);
            return Ok(channel);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<ChannelResponse>), StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllChannelsAsync()
        {
            var channels = await _channelService.GetAllChannelsAsync();
            return Ok(channels);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<ChannelResponse>), StatusCodes.Status200OK)]
        [Route("/api/me/channel")]
        public async Task<IActionResult> GetMyChannelsAsync()
        {
            var channels = await _channelService.GetUserChannelsAsync(new UserRequest(GetCurrentSaasUserId()));
            return Ok(channels);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<ChannelSummaryResponse>), StatusCodes.Status200OK)]
        [Route("allowed")]
        public async Task<IActionResult> GetAllowedChannelsAsync()
        {
            var channels = await _channelService.GetAllowedChannelsAsync(new UserRequest(GetCurrentSaasUserId()));
            return Ok(channels);
        }

        [HttpPut]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/close")]
        public async Task<IActionResult> CloseChannelAsync(Guid channelId)
        {
            await _channelSocketService.CloseChannelAsync(new ChannelRequest(GetCurrentSaasUserId(), channelId));
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<MemberSummary>), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/participant")]
        public async Task<IActionResult> GetChannelParticipantsAsync(Guid channelId)
        {
            var members = await _memberService.GetChannelMembersAsync(channelId);
            return Ok(members);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ChannelResponse), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/invite/{memberId:guid}")]
        public async Task<IActionResult> InviteMemberAsync(Guid channelId, Guid memberId)
        {
            var response = await _channelSocketService.InviteMemberAsync(new InviteMemberRequest(GetCurrentSaasUserId(), channelId, memberId));
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ChannelResponse), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/invite/member")]
        public async Task<IActionResult> InviteMultipleMembersAsync([FromBody] InviteMembersRequest request, Guid channelId)
        {
            request.SaasUserId = GetCurrentSaasUserId();
            request.ChannelId = channelId;
            var channel = await _channelSocketService.InviteMultipleMembersAsync(request);
            return Ok(channel);
        }

        [HttpGet]
        [ProducesResponseType(typeof(SettingsResponse), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/settings")]
        public async Task<IActionResult> GetChannelSettingsAsync(Guid channelId)
        {
            var settings = await _channelService.GetChannelSettingsAsync(channelId);
            return Ok(settings);
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/join")]
        public async Task<IActionResult> JoinToChannelAsync(Guid channelId)
        {
            await _channelSocketService.JoinToChannelAsync(new JoinToChannelRequest(GetCurrentSaasUserId(), channelId));
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/leave")]
        public async Task<IActionResult> LeaveChannelAsync(Guid channelId)
        {
            await _channelService.RemoveMemberFromChannelAsync(new ChannelRequest(GetCurrentSaasUserId(), channelId));
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/delete/{memberId:guid}")]
        public async Task<IActionResult> DeleteMemberAsync(Guid channelId, Guid memberId)
        {
            await _channelSocketService.DeleteMemberAsync(new DeleteMemberRequest(GetCurrentSaasUserId(), channelId, memberId));
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/mute")]
        public async Task<IActionResult> MuteChannelAsync(Guid channelId)
        {
            await _channelSocketService.MuteChannelAsync(new ChannelRequest(GetCurrentSaasUserId(), channelId));
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/message/count")]
        public async Task<IActionResult> GetChannelMessagesCountAsync(Guid channelId)
        {
            var messagesCount = await _channelService.GetChannelMessagesCountAsync(channelId);
            return Ok(messagesCount);
        }
    }
}