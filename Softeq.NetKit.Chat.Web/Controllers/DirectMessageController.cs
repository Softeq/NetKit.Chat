// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectChannel;
using Softeq.NetKit.Chat.SignalR.Sockets;
using Softeq.NetKit.Chat.Web.TransportModels.Request.DirectChannel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using UpdateDirectMessageRequest = Softeq.NetKit.Chat.Web.TransportModels.Request.DirectChannel.UpdateDirectMessageRequest;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/directmessages")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    public class DirectMessageController : BaseApiController
    {
        private readonly IDirectMessageSocketService _directMessageSocketService;
        private readonly IDirectChannelService _directChannelService;

        public DirectMessageController(IDirectMessageSocketService directMessageSocketService, IDirectChannelService directChannelService)
        {
            Ensure.That(directMessageSocketService).IsNotNull();
            Ensure.That(directChannelService).IsNotNull();

            _directMessageSocketService = directMessageSocketService;
            _directChannelService = directChannelService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddDirectMessageAsync([FromBody] AddDirectMessageRequest request)
        {
            var addDirectMessageRequest = new CreateDirectMessageRequest(GetCurrentSaasUserId(), request.DirectChannelId, request.Type, request.Body);
            var directMessageResponse = await _directMessageSocketService.AddMessageAsync(addDirectMessageRequest);

            return Ok(directMessageResponse);
        }

        [HttpPut]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [Route("{channelId:guid}")]
        public async Task<IActionResult> UpdateDirectMessageAsync(Guid channelId, [FromBody] UpdateDirectMessageRequest request)
        {
            var updateDirectMessageRequest = new Domain.TransportModels.Request.DirectChannel.UpdateDirectMessageRequest(GetCurrentSaasUserId(),
                    request.MessageId, request.DirectChannelId, request.Type, request.Body);
            var directMessageResponse = await _directMessageSocketService.UpdateMessageAsync(updateDirectMessageRequest);

            return Ok(directMessageResponse);
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/delete/{memberId:guid}")]
        public async Task<IActionResult> DeleteDirectMessageAsync(Guid messageId, Guid directChannelId)
        {
            var directMessageResponse = await _directMessageSocketService.DeleteMessageAsync(GetCurrentSaasUserId(), messageId, directChannelId);

            return Ok(directMessageResponse);
        }

        [HttpGet]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/settings")]
        public async Task<IActionResult> GetDirectMessageByIdAsync(Guid messageId)
        {
            var directMessageResponse = await _directChannelService.GetMessagesByIdAsync(messageId);

            return Ok(directMessageResponse);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<MessageResponse>), StatusCodes.Status200OK)]
        [Route("{channelId:guid}/settings")]
        public async Task<IActionResult> GetDirectMessageByChannelIdAsync(Guid channelId)
        {
            var directMessageResponse = await _directChannelService.GetMessagesByChannelIdAsync(channelId);

            return Ok(directMessageResponse);
        }
    }
}
