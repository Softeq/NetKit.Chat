// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectMembers;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/direct")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    public class DirectMembersController : BaseApiController
    {
        private readonly IDirectMemberService _directMemberService;

        public DirectMembersController(IDirectMemberService directMemberService)
        {
            _directMemberService = directMemberService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CreateDirectMembersResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateDirectMembersAsync([FromBody] TransportModels.Request.DirectMembers.CreateDirectMembersRequest request)
        {
            var createDirectMembersRequest = new CreateDirectMembersRequest(request.FirstMemberId, request.SecondMemberId, GetCurrentSaasUserId());

            await _directMemberService.CreateDirectMembers(createDirectMembersRequest);

            return Ok();
        }
    }
}
