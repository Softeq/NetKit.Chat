// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectMembers;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers;
using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.SignalR.Sockets;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/direct")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    public class DirectMembersController : BaseApiController
    {
        private readonly IDirectMessageSocketService _directMessageSocketService;

        public DirectMembersController(IDirectMessageSocketService directMessageSocketService)
        {
            _directMessageSocketService = directMessageSocketService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateDirectMembersResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateDirectMembersAsync([FromBody] TransportModels.Request.DirectMembers.CreateDirectMembersRequest request)
        {
            var createDirectMembersRequest = new CreateDirectMembersRequest(GetCurrentSaasUserId(), request.FirstMemberId, request.SecondMemberId)
            {
                DirectId = Guid.NewGuid()
            };

           var createDirectMembersResponse = await _directMessageSocketService.CreateDirectMembers(createDirectMembersRequest);

            return Ok(createDirectMembersResponse);
        }
    }
}
