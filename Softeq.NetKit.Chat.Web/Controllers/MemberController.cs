// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/member")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    [ProducesResponseType(typeof(List<ErrorDto>), 400)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    public class MemberController : BaseApiController
    {
        private readonly IMemberService _memberService;

        public MemberController(
            ILogger logger,
            IMemberService memberService) : base(logger)
        {
            _memberService = memberService;
        }

        [HttpGet]
        [Route("/api/me/member")]
        [ProducesResponseType(typeof(MemberSummary), 200)]
        public async Task<IActionResult> GetMemberAsync()
        {
            var saasUserId = GetCurrentSaasUserId();
            var result = await _memberService.GetMemberSummaryBySaasUserIdAsync(saasUserId);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MemberSummary>), 200)]
        public async Task<IActionResult> GetMembersAsync()
        {
            var result = await _memberService.GetAllMembersAsync();
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MemberSummary), 200)]
        public async Task<IActionResult> AddMemberAsync()
        {
            var userId = GetCurrentSaasUserId();
            var email = GetCurrentUserEmail();
            var member = await _memberService.AddMemberAsync(userId, email);
            return Ok(member);
        }
    }
}
