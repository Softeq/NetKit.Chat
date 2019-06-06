// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Chat.Client.SDK.Enums;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Member;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Settings;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Settings;
using Softeq.NetKit.Chat.Notifications;
using Softeq.NetKit.Chat.Notifications.Services;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Request;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/member")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    public class MemberController : BaseApiController
    {
        private readonly IMemberService _memberService;
        private readonly IChannelService _channelService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly INotificationSettingsService _notificationSettingsService;

        public MemberController(IMemberService memberService,
            IChannelService channelService,
            IPushNotificationService pushNotificationService,
            INotificationSettingsService notificationSettingsService)
        {
            _memberService = memberService;
            _channelService = channelService;
            _pushNotificationService = pushNotificationService;
            _notificationSettingsService = notificationSettingsService;
        }

        [HttpGet]
        [Route("/api/me/member")]
        [ProducesResponseType(typeof(MemberSummaryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMemberAsync()
        {
            var result = await _memberService.GetMemberBySaasUserIdAsync(GetCurrentSaasUserId());
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedMembersResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPagedMembersAsync(int pageNumber, int pageSize, string nameFilter)
        {
            var result = await _memberService.GetPagedMembersAsync(pageNumber, pageSize, nameFilter, GetCurrentSaasUserId());
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MemberSummaryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddMemberAsync()
        {
            var userId = GetCurrentSaasUserId();
            var email = GetCurrentUserEmail();
            var result = await _memberService.AddMemberAsync(userId, email);
            return Ok(result);
        }

        [HttpPost]
        [Route("activate")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> ActivateUserAsync()
        {
            var userId = GetCurrentSaasUserId();

            await _memberService.ActivateMemberAsync(userId);

            return Ok();
        }

        [HttpPost]
        [Route("/api/me/notifications/subscribe")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> SubscribeUserOnChannelsAsync()
        {
            var userId = GetCurrentSaasUserId();

            var memberChannels = await _channelService.GetMemberChannelsAsync(userId);

            await _pushNotificationService.SubscribeUserOnTagsAsync(userId, memberChannels.Select(x => PushNotificationsTagTemplates.GetChatChannelTag(x.Id.ToString())));

            return Ok();
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

        #region Notifications
        
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [HttpPost]
        [Route("push-token/subscribe")]
        public async Task<IActionResult> SubscribePushTokenAsync([FromBody]PushTokenRequest model)
        {
            var userId = GetCurrentSaasUserId();

            var token = model.Token.Trim().Replace(" ", string.Empty);

            await _pushNotificationService.CreateOrUpdatePushSubscriptionAsync(new CreatePushTokenRequest(userId)
            {
                Token = token,
                DevicePlatform = model.DevicePlatform
            });

            return Ok();
        }

        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [Route("push-token/unsubscribe")]
        [HttpPost]
        public async Task<IActionResult> UnsubscribePushTokenAsync([FromBody]PushTokenRequest model)
        {
            var userId = GetCurrentSaasUserId();

            if (model != null)
            {
                var token = model.Token.Trim().Replace(" ", string.Empty);

                await _pushNotificationService.UnsubscribeDeviceFromPushAsync(new CreatePushTokenRequest(userId)
                {
                    Token = token,
                    DevicePlatform = model.DevicePlatform
                });
            }

            return Ok();
        }

        #endregion
    }
}