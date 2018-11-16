// Developed by Softeq Development Corporation
// http://www.softeq.com
 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Softeq.NetKit.Chat.Domain.Channel;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling;
using Softeq.NetKit.Chat.Domain.Settings.TransportModels.Response;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/channel")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    [ProducesResponseType(typeof(List<ErrorDto>), 400)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    public class ChannelController : BaseApiController
    {
        private readonly IChannelService _channelService;
        private readonly IMemberService _memberService;

        public ChannelController(
            ILogger logger, 
            IChannelService channelService, 
            IMemberService memberService) : base(logger)
        {
            _channelService = channelService;
            _memberService = memberService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ChannelResponse), 200)]
        [Route("{channelId:guid}")]
        public async Task<IActionResult> GetChannelInfoByIdAsync(Guid channelId)
        {
            var userId = GetCurrentUserId();
            var channel = await _channelService.GetChannelByIdAsync(new ChannelRequest(userId, channelId));
            return Ok(channel);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ChannelSummaryResponse), 200)]
        public async Task<IActionResult> CreateChannelAsync([FromBody] CreateChannelRequest request)
        {
            var userId = GetCurrentUserId();
            request.SaasUserId = userId;
            var channel = await _channelService.CreateChannelAsync(request);
            return Ok(channel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ChannelResponse), 200)]
        [Route("{channelId:guid}")]
        public async Task<IActionResult> UpdateChannelAsync(Guid channelId, [FromBody] UpdateChannelRequest request)
        {
            request.ChannelId = channelId;
            request.SaasUserId = GetCurrentUserId();
            var channel = await _channelService.UpdateChannelAsync(request);
            return Ok(channel);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ChannelResponse>), 200)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllChannelsAsync()
        {
            var channels = await _channelService.GetAllChannelsAsync();
            return Ok(channels);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ChannelResponse>), 200)]
        [Route("/api/me/channel")]
        public async Task<IActionResult> GetMyChannelsAsync()
        {
            var userId = GetCurrentUserId();
            var channels = await _channelService.GetMyChannelsAsync(new UserRequest(userId));
            return Ok(channels);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ChannelSummaryResponse>), 200)]
        [Route("allowed")]
        public async Task<IActionResult> GetAllowedChannelsAsync()
        {
            var userId = GetCurrentUserId();
            var channels = await _channelService.GetAllowedChannelsAsync(new UserRequest(userId));
            return Ok(channels);
        }

        [HttpPut]
        [ProducesResponseType(typeof(void), 200)]
        [Route("{channelId:guid}/close")]
        public async Task<IActionResult> CloseChannelAsync(Guid channelId)
        {
            var userId = GetCurrentUserId();
            await _channelService.CloseChannelAsync(new ChannelRequest(userId, channelId));
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ParticipantResponse>), 200)]
        [Route("{channelId:guid}/participant")]
        public async Task<IActionResult> GetChannelParticipantsAsync(Guid channelId)
        {
            var userId = GetCurrentUserId();
            var members = await _memberService.GetChannelMembersAsync(new ChannelRequest(userId, channelId));
            return Ok(members);
        }       

        [HttpPost]
        [ProducesResponseType(typeof(ChannelResponse), 200)]
        [Route("{channelId:guid}/invite/{memberId:guid}")]
        public async Task<IActionResult> InviteMemberAsync(Guid channelId, Guid memberId)
        {
            var userId = GetCurrentUserId();
            var res = await _memberService.InviteMemberAsync(new InviteMemberRequest(userId, channelId, memberId));
            return Ok(res);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ChannelSummaryResponse), 200)]
        [Route("{channelId:guid}/invite/member")]
        public async Task<IActionResult> InviteMembersAsync([FromBody] InviteMembersRequest request, Guid channelId)
        {
            var userId = GetCurrentUserId();
            request.SaasUserId = userId;
            request.ChannelId = channelId;
            var channel = await _memberService.InviteMultipleMembersAsync(request);
            return Ok(channel);
        }

        [HttpGet]
        [ProducesResponseType(typeof(SettingsResponse), 200)]
        [Route("{channelId:guid}/settings")]
        public async Task<IActionResult> GetChannelSettingsAsync(Guid channelId)
        {
            var settings = await _channelService.GetChannelSettingsAsync(channelId);
            return Ok(settings);
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), 200)]
        [Route("{channelId:guid}/join")]
        public async Task<IActionResult> JoinToChannelAsync(Guid channelId)
        {
            var userId = GetCurrentUserId();
            await _channelService.JoinToChannelAsync(new JoinToChannelRequest(userId, channelId));
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), 200)]
        [Route("{channelId:guid}/leave")]
        public async Task<IActionResult> LeaveChannelAsync(Guid channelId)
        {
            var userId = GetCurrentUserId();
            await _channelService.LeaveChannelAsync(new ChannelRequest(userId, channelId));
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), 200)]
        [Route("{channelId:guid}/mute")]
        public async Task<IActionResult> MuteChannelAsync(Guid channelId)
        {
            var userId = GetCurrentUserId();
            await _channelService.MuteChannelAsync(new ChannelRequest(userId, channelId));
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(int), 200)]
        [Route("{channelId:guid}/message/count")]
        public async Task<IActionResult> GetChannelMessagesCountAsync(Guid channelId)
        {
            var userId = GetCurrentUserId();
            var messagesCount = await _channelService.GetChannelMessageCountAsync(new ChannelRequest(userId, channelId));
            return Ok(messagesCount);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{channelId:guid}/delete/{saasUserId}")]
        public async Task<IActionResult> DeleteMemberFromChannelAsync(Guid channelId, string saasUserId)
        {
            if (string.IsNullOrWhiteSpace(saasUserId))
            {
                return BadRequest($"'{nameof(saasUserId)}' is required. It cannot be null or empty.");
            }
            
            await _channelService.DeleteMemberFromChannelAsync(new DeleteMemberFromChannelRequest(saasUserId, channelId));
            return Ok();
        }
    }
}