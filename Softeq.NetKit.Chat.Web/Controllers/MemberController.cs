// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/member")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    public class MemberController : BaseApiController
    {
        private readonly IMemberService _memberService;

        public MemberController(ILogger logger, IMemberService memberService) 
            : base(logger)
        {
            _memberService = memberService;
        }

        [HttpGet]
        [Route("/api/me/member")]
        [ProducesResponseType(typeof(MemberSummary), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMemberAsync()
        {
            var result = await _memberService.GetMemberBySaasUserIdAsync(GetCurrentSaasUserId());
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedMembersResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedMembersAsync(int pageNumber, int pageSize)
        {
            var result = await _memberService.GetPagedMembersAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MemberSummary), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddMemberAsync()
        {
            var userId = GetCurrentSaasUserId();
            var email = GetCurrentUserEmail();
            var result = await _memberService.AddMemberAsync(userId, email);
            return Ok(result);
        }

        private string GetCurrentUserEmail()
        {
            return User.FindFirstValue(JwtClaimTypes.Name);
        }
    }
}