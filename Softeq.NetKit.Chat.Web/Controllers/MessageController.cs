// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Softeq.NetKit.Chat.Domain.Message;
using Softeq.NetKit.Chat.Domain.Message.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Message.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling;
using Softeq.NetKit.Chat.Web.Common;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/channel/{channelId:guid}/message")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin, User")]
    [ProducesResponseType(typeof(List<ErrorDto>), 400)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    public class MessageController : BaseApiController
    {
        private readonly IMessageService _messageService;
        public MessageController(ILogger logger, IMessageService messageService) : base(logger)
        {
            _messageService = messageService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(MessageResponse), 200)]
        [Route("")]
        public async Task<IActionResult> CreateMessageAsync(Guid channelId, [FromBody] CreateMessageRequest request)
        {
            request.ChannelId = channelId;
            request.SaasUserId = GetCurrentUserId();
            var message = await _messageService.CreateMessageAsync(request);
            return Ok(message);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(void), 200)]
        [Route("{messageId:guid}")]
        public async Task<IActionResult> DeleteMessageAsync(Guid channelId, Guid messageId)
        {
            var userId = GetCurrentUserId();
            await _messageService.DeleteMessageAsync(new DeleteMessageRequest(userId, messageId));
            return Ok();
        }

        [HttpPut]
        [ProducesResponseType(typeof(MessageResponse), 200)]
        [Route("{messageId:guid}")]
        public async Task<IActionResult> UpdateMessageAsync(Guid messageId, [FromBody] UpdateMessageRequest request)
        {
            var userId = GetCurrentUserId();
            var message = await _messageService.UpdateMessageAsync(
                new UpdateMessageRequest(userId, messageId, request.Body));
            return Ok(message);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MessageResponse), 200)]
        [Route("{messageId:guid}/attachment")]
        public async Task<IActionResult> AddMessageAttachmentAsync(Guid messageId, IFormCollection model)
        {
            var userId = GetCurrentUserId();
            if (model?.Files == null)
            {
                return BadRequest(new ErrorDto(ErrorCode.NotFound, "There is not photo"));
            }
            var file = model.Files.Count == 0 ? null : model.Files.First();
            if (file == null)
            {
                return BadRequest(new ErrorDto(ErrorCode.NotFound, "There is no files in the request"));
            }
            var type = AttachmentsUtils.GetExtentionFromMimeType(file.ContentType);
            if (type != "jpg" && type != "png" && type != "mp4")
            {
                return BadRequest(new Exception("Only jpg, png, mp4 formats are supported."));
            }
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    await _messageService.AddMessageAttachmentAsync(new AddMessageAttachmentRequest(userId, messageId, stream, type, file.ContentType, file.Length));
                }
            }
            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType(typeof(void), 200)]
        [Route("{messageId:guid}/attachment/{attachmentId:guid}")]
        public async Task<IActionResult> DeleteMessageAttachmentAsync(Guid messageId, Guid attachmentId)
        {
            var userId = GetCurrentUserId();
            await _messageService.DeleteMessageAttachmentAsync(new DeleteMessageAttachmentRequest(userId, messageId, attachmentId));
            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(void), 200)]
        [Route("{messageId:guid}/mark-as-read")]
        public async Task<IActionResult> AddLastReadMessageAsync(Guid messageId, Guid channelId)
        {
            var userId = GetCurrentUserId();
            await _messageService.AddLastReadMessageAsync(new AddLastReadMessageRequest(channelId, messageId, userId));
            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(typeof(MessagesResult), 200)]
        [Route("old")]
        public async Task<IActionResult> GetReadMessagesAsync(Guid channelId, [FromQuery] Guid messageId, [FromQuery] DateTimeOffset messageCreated,  [FromQuery] int? pageSize)
        {
            var userId = GetCurrentUserId();
            var messages = await _messageService.GetOlderMessagesAsync(new GetMessagesRequest(userId, channelId, messageId, messageCreated, pageSize));
            return Ok(messages);
        }

        [HttpGet]
        [ProducesResponseType(typeof(MessagesResult), 200)]
        public async Task<IActionResult> GetMessagesAsync(Guid channelId, [FromQuery] Guid messageId, [FromQuery] DateTimeOffset messageCreated, [FromQuery] int? pageSize)
        {
            var userId = GetCurrentUserId();
            var messages = await _messageService.GetMessagesAsync(new GetMessagesRequest(userId, channelId, messageId, messageCreated, pageSize));
            return Ok(messages);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<Guid>), 200)]
        public async Task<IActionResult> SearchMessagesAsync(Guid channelId, [FromQuery] string searchText)
        {
            var messageIds = await _messageService.FindMessageIdsAsync(channelId, searchText);
            return Ok(messageIds);
        }

        [HttpGet]
        [ProducesResponseType(typeof(MessagesResult), 200)]
        [Route("last")]
        public async Task<IActionResult> GetLastMessagesAsync(Guid channelId)
        {
            var userId = GetCurrentUserId();
            var messages = await _messageService.GetLastMessagesAsync(new GetLastMessagesRequest(userId, channelId));
            return Ok(messages);
        }
    }
}