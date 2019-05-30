// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Settings;
using Softeq.NetKit.Chat.SignalR.Sockets;
using WebRequest = Softeq.NetKit.Chat.Web.TransportModels.Request;
using Model = Softeq.NetKit.Chat.Client.SDK.REST.Models.CommonModels.Request.Channel;

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

        public ChannelController(IChannelService channelService, IChannelSocketService channelSocketService, IMemberService memberService)
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
        public async Task<IActionResult> CreateChannelAsync([FromBody] TransportModels.Request.Channel.CreateChannelRequest request)
        {
            var createChannelRequest = new CreateChannelRequest(GetCurrentSaasUserId(), request.Name, request.Type)
            {
                AllowedMembers = request.AllowedMembers,
                Description = request.Description,
                PhotoUrl = request.PhotoUrl,
                WelcomeMessage = request.WelcomeMessage
            };
            var channel = await _channelSocketService.CreateChannelAsync(createChannelRequest);
            return Ok(channel);
        }


        [HttpPost]
        [ProducesResponseType(typeof(ChannelSummaryResponse), StatusCodes.Status200OK)]
        [Route("direct")]
        public async Task<IActionResult> CreateDirectChannelAsync([FromBody] CreateDirectChannelRequest request)
        {
            var channel = await _channelSocketService.CreateDirectChannelAsync(new CreateDirectChannelRequest(GetCurrentSaasUserId(), request.MemberId));
            return Ok(channel);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ChannelSummaryResponse), StatusCodes.Status200OK)]
        [Route("{channelId:guid}")]
        public async Task<IActionResult> UpdateChannelAsync(Guid channelId, [FromBody] TransportModels.Request.Channel.UpdateChannelRequest request)
        {
            var updateChannelRequest = new UpdateChannelRequest(GetCurrentSaasUserId(), channelId, request.Name)
            {
                PhotoUrl = request.PhotoUrl,
                Description = request.Description,
                WelcomeMessage = request.WelcomeMessage
            };
            var channel = await _channelSocketService.UpdateChannelAsync(updateChannelRequest);
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
            var channels = await _channelService.GetMemberChannelsAsync(GetCurrentSaasUserId());
            return Ok(channels);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<ChannelSummaryResponse>), StatusCodes.Status200OK)]
        [Route("allowed")]
        public async Task<IActionResult> GetAllowedChannelsAsync()
        {
            var channels = await _channelService.GetAllowedChannelsAsync(GetCurrentSaasUserId());
            return Ok(channels);
        }

        [HttpPut]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/close")]
        public async Task<IActionResult> CloseChannelAsync(Model.ChannelRequest channelRequest)
        {
            var request = new ChannelRequest(GetCurrentSaasUserId(), channelRequest.ChannelId);
            await _channelSocketService.CloseChannelAsync(request);

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<MemberSummaryResponse>), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/participant")]
        public async Task<IActionResult> GetChannelParticipantsAsync(Guid channelId)
        {
            var members = await _memberService.GetChannelMembersAsync(channelId);
            return Ok(members);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedMembersResponse), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/invite/user")]
        public async Task<IActionResult> GetPotentialChannelMembersAsync(Guid channelId, int pageNumber, int pageSize, string nameFilter)
        {
            var result = await _memberService.GetPotentialChannelMembersAsync(channelId, new GetPotentialChannelMembersRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                NameFilter = nameFilter
            });

            return Ok(result);
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
        public async Task<IActionResult> InviteMultipleMembersAsync(Guid channelId, [FromBody] TransportModels.Request.Member.InviteMultipleMembersRequest request)
        {
            var inviteMultipleMembersRequest = new InviteMultipleMembersRequest(GetCurrentSaasUserId(), channelId, request.InvitedMembersIds);
            var channel = await _channelSocketService.InviteMultipleMembersAsync(inviteMultipleMembersRequest);
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
        public async Task<IActionResult> JoinToChannelAsync(Model.ChannelRequest request)
        {
            var channelRequest = new ChannelRequest(GetCurrentSaasUserId(), request.ChannelId);
            await _channelSocketService.JoinToChannelAsync(channelRequest);

            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/leave")]
        public async Task<IActionResult> LeaveChannelAsync(Guid channelId)
        {
            await _channelSocketService.LeaveChannelAsync(new ChannelRequest(GetCurrentSaasUserId(), channelId));
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/delete/{memberId:guid}")]
        public async Task<IActionResult> DeleteMemberFromChannelAsync(Guid channelId, Guid memberId)
        {
            await _channelSocketService.DeleteMemberFromChannelAsync(new DeleteMemberRequest(GetCurrentSaasUserId(), channelId, memberId));
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/pin")]
        public async Task<IActionResult> PinChannelAsync(Guid channelId)
        {
            await _channelService.PinChannelAsync(GetCurrentSaasUserId(), channelId, true);
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/unpin")]
        public async Task<IActionResult> UnpinChannelAsync(Guid channelId)
        {
            await _channelService.PinChannelAsync(GetCurrentSaasUserId(), channelId, false);
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/mute")]
        public async Task<IActionResult> MuteChannelAsync(Guid channelId)
        {
            await _channelSocketService.MuteChannelAsync(new MuteChannelRequest
            {
                SaasUserId = GetCurrentSaasUserId(),
                ChannelId = channelId,
                IsMuted = true
            });

            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/unmute")]
        public async Task<IActionResult> UnmuteChannelAsync(Guid channelId)
        {
            await _channelSocketService.MuteChannelAsync(new MuteChannelRequest
            {
                SaasUserId = GetCurrentSaasUserId(),
                ChannelId = channelId,
                IsMuted = false
            });

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
