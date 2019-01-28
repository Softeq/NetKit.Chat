// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectMembers;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers;
using Softeq.NetKit.Chat.SignalR.Sockets;
using System;
using System.Threading.Tasks;
using EnsureThat;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/direct")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    public class DirectMembersController : BaseApiController
    {
        private readonly IDirectMessageSocketService _directMessageSocketService;
        private readonly IDirectMessageService _directMessageService;

        public DirectMembersController(IDirectMessageSocketService directMessageSocketService, IDirectMessageService directMessageService)
        {
            Ensure.That(directMessageSocketService).IsNotNull();
            Ensure.That(directMessageService).IsNotNull();

            _directMessageSocketService = directMessageSocketService;
            _directMessageService = directMessageService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DirectChannelResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDirectMembersByIdAsync([FromBody] Guid id)
        {
            var directMembersResponse = await _directMessageService.GetDirectMembersById(id);

            return Ok(directMembersResponse);
        }

        [HttpPost]
        [ProducesResponseType(typeof(DirectChannelResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateDirectMembersAsync([FromBody] TransportModels.Request.DirectMembers.CreateDirectMembersRequest request)
        {
            var createDirectMembersRequest = new CreateDirectMembersRequest(GetCurrentSaasUserId(), request.OwnerId, request.MemberId)
            {
                DirectMembersId = Guid.NewGuid()
            };

            var createDirectMembersResponse = await _directMessageSocketService.CreateDirectMembers(createDirectMembersRequest, HttpContext.Connection.Id);

            return Ok(createDirectMembersResponse);
        }
    }
}
