// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Settings;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Settings;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/member")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    public class MemberController : BaseApiController
    {
        private readonly IMemberService _memberService;
        private readonly INotificationSettingsService _notificationSettingsService;

        public MemberController(IMemberService memberService, INotificationSettingsService notificationSettingsService)
        {
            _memberService = memberService;
            _notificationSettingsService = notificationSettingsService;
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
        public async Task<IActionResult> GetPagedMembersAsync(int pageNumber, int pageSize, string nameFilter)
        {
            var result = await _memberService.GetPagedMembersAsync(pageNumber, pageSize, nameFilter);
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

        #region Settings

        [HttpGet]
        [ProducesResponseType(typeof(NotificationSettingResponse), StatusCodes.Status200OK)]
        [Route("/api/me/settings/notification")]
        public async Task<IActionResult> GetUserNotificationSettingsAsync()
        {
            var userId = GetCurrentSaasUserId();
            var res = await _notificationSettingsService.GetUserNotificationSettingsAsync(new UserRequest(userId));
            return Ok(res);
        }

        [HttpPost]
        [HttpPut]
        [ProducesResponseType(typeof(NotificationSettingResponse), StatusCodes.Status200OK)]
        [Route("/api/me/settings/notification/{key}")]
        public async Task<IActionResult> UpdateUserNotificationSettingsAsync(NotificationSettingKey key, [FromBody] NotificationSettingValue value)
        {
            var userId = GetCurrentSaasUserId();
            var res = await _notificationSettingsService.UpdateUserNotificationSettingsAsync(new NotificationSettingRequest(userId)
            {
                Key = key,
                Value = value
            });
            return Ok(res);
        }

        #endregion

        private string GetCurrentUserEmail()
        {
            return User.FindFirstValue(JwtClaimTypes.Name);
        }
    }
}