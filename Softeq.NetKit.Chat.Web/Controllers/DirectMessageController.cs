// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;
using Softeq.NetKit.Chat.SignalR.Sockets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/channel")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    public class DirectMessageController : BaseApiController
    {
        private readonly IDirectMessageSocketService _directMessageSocketService;

        public DirectMessageController(IDirectMessageSocketService directMessageSocketService)
        {
            Ensure.That(directMessageSocketService).IsNotNull();

            _directMessageSocketService = directMessageSocketService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(DirectMessageResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddDirectMessageAsync([FromBody] TransportModels.Request.Channel.UpdateChannelRequest request)
        {
            // TODO
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(typeof(DirectMessageResponse), StatusCodes.Status200OK)]
        [Route("{channelId:guid}")]
        public async Task<IActionResult> UpdateDirectMessageAsync(Guid channelId, [FromBody] TransportModels.Request.Channel.UpdateChannelRequest request)
        {
            // TODO
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/delete/{memberId:guid}")]
        public async Task<IActionResult> DeleteDirectMessageAsync(Guid channelId, Guid memberId)
        {
            // TODO
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(DirectMessageResponse), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/settings")]
        public async Task<IActionResult> GetDirectMessageByIdAsync(Guid messageId)
        {
            // TODO
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<DirectMessageResponse>), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/settings")]
        public async Task<IActionResult> GetDirectMessageByChannelIdAsync(Guid channelId)
        {
            // TODO
            return Ok();
        }
    }
}
