// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.MessageAttachment;
using Softeq.NetKit.Chat.SignalR.Sockets;
using Softeq.NetKit.Chat.Web.Common;
using WebRequest = Softeq.NetKit.Chat.Web.TransportModels.Request;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/channel/{channelId:guid}/message")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin, User")]
    public class MessageController : BaseApiController
    {
        private readonly IMessageService _messageService;
        private readonly IMessageSocketService _messageSocketService;

        public MessageController(IMessageService messageService, IMessageSocketService messageSocketService)
        {
            Ensure.That(messageService).IsNotNull();
            Ensure.That(messageSocketService).IsNotNull();

            _messageService = messageService;
            _messageSocketService = messageSocketService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [Route("")]
        public async Task<IActionResult> AddMessageAsync(Guid channelId, [FromBody] WebRequest.Message.AddMessageRequest request)
        {
            var createMessageRequest = new CreateMessageRequest(GetCurrentSaasUserId(), channelId, request.Type, request.Body)
            {
                ForwardedMessageId = request.ForwardedMessageId,
                ImageUrl = request.ImageUrl
            };
            var result = await _messageSocketService.AddMessageAsync(createMessageRequest, request.ClientConnectionId);
            return Ok(result);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{messageId:guid}")]
        public async Task<IActionResult> DisableMessageAsync(Guid channelId, Guid messageId)
        {
            await _messageSocketService.DisableMessageAsync(new DisabledMessageRequest(GetCurrentSaasUserId(), messageId));
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(typeof(MessageResponse), StatusCodes.Status200OK)]
        [Route("{messageId:guid}")]
        public async Task<IActionResult> UpdateMessageAsync(Guid messageId, [FromBody] WebRequest.Message.UpdateMessageRequest request)
        {
            var result = await _messageSocketService.UpdateMessageAsync(new UpdateMessageRequest(GetCurrentSaasUserId(), messageId, request.Body));
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(AttachmentResponse), StatusCodes.Status200OK)]
        [Route("{messageId:guid}/attachment")]
        public async Task<IActionResult> AddMessageAttachmentAsync(Guid messageId, IFormCollection model)
        {
            // TODO: Improve validation to validate both WebAPI and SignalR request models in the same place.
            var file = model?.Files?.FirstOrDefault();
            if (file == null || file.Length <= 0)
            {
                return BadRequest("Attached file can not be empty");
            }

            // TODO: Replace this helper by some NuGet or smth else.
            var extension = AttachmentsUtils.GetExtensionFromMimeType(file.ContentType);
            var supportedExtensions = new[] { "jpg", "png", "mp4" };
            if (extension == null || !supportedExtensions.Contains(extension))
            {
                return BadRequest($"Only {string.Join(", ", supportedExtensions)} formats are supported");
            }

            using (var stream = file.OpenReadStream())
            {
                var request = new AddMessageAttachmentRequest(GetCurrentSaasUserId(), messageId, stream, extension, file.ContentType, file.Length);
                var result = await _messageSocketService.AddMessageAttachmentAsync(request);
                return Ok(result);
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{messageId:guid}/attachment/{attachmentId:guid}")]
        public async Task<IActionResult> DeleteMessageAttachmentAsync(Guid messageId, Guid attachmentId)
        {
            var request = new DeleteMessageAttachmentRequest(GetCurrentSaasUserId(), messageId, attachmentId);
            await _messageSocketService.DeleteMessageAttachmentAsync(request);
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("{messageId:guid}/mark-as-read")]
        public async Task<IActionResult> MarkAsReadMessageAsync(Guid messageId, Guid channelId)
        {
            await _messageSocketService.SetLastReadMessageAsync(new SetLastReadMessageRequest(GetCurrentSaasUserId(), channelId, messageId));
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(MessagesResult), StatusCodes.Status200OK)]
        [Route("old")]
        public async Task<IActionResult> GetReadMessagesAsync(Guid channelId, [FromQuery] Guid messageId, [FromQuery] DateTimeOffset messageCreated,  [FromQuery] int? pageSize)
        {
            var request = new GetMessagesRequest(GetCurrentSaasUserId(), channelId, messageId, messageCreated, pageSize);
            var result = await _messageService.GetOlderMessagesAsync(request);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(MessagesResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMessagesAsync(Guid channelId, [FromQuery] Guid messageId, [FromQuery] DateTimeOffset messageCreated, [FromQuery] int? pageSize)
        {
            var request = new GetMessagesRequest(GetCurrentSaasUserId(), channelId, messageId, messageCreated, pageSize);
            var result = await _messageService.GetMessagesAsync(request);
            return Ok(result);
        }

        [HttpGet]
        [Route("search")]
        [ProducesResponseType(typeof(IReadOnlyCollection<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchMessagesAsync(Guid channelId, [FromQuery] string searchText)
        {
            var messageIds = await _messageService.FindMessageIdsAsync(channelId, searchText);
            return Ok(messageIds);
        }

        [HttpGet]
        [ProducesResponseType(typeof(MessagesResult), StatusCodes.Status200OK)]
        [Route("last")]
        public async Task<IActionResult> GetLastMessagesAsync(Guid channelId)
        {
            var result = await _messageService.GetLastMessagesAsync(new GetLastMessagesRequest(GetCurrentSaasUserId(), channelId));
            return Ok(result);
        }
    }
}