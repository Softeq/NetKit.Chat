using System.Threading.Tasks;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Chat.Notifications.Services;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Request;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Response;
using Softeq.NetKit.Chat.Notifications.TransportModels.Shared.Request;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/notifications")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin, User")]
    public class NotificationController : BaseApiController
    {
        private readonly INotificationService _notificationService;
        public NotificationController(ILogger logger, INotificationService notificationService) : base(logger)
        {
            _notificationService = notificationService;
        }

        [ProducesResponseType(typeof(void), 200)]
        [HttpPost]
        [Route("/api/notification/{type}")]
        public async Task<IActionResult> CreateNotificationAsync(NotificationType type, [FromBody] dynamic notification)
        {
            var request = new CreateNotificationRequest(type, notification);
            await _notificationService.PostNotificationAsync(request);
            return Ok();
        }

        [ProducesResponseType(typeof(void), 200)]
        [HttpDelete]
        public async Task<IActionResult> DeleteNotificationsAsync()
        {
            var userId = GetCurrentSaasUserId();
            await _notificationService.RemoveNotificationsAsync(new UserRequest(userId));
            return Ok();
        }

        [ProducesResponseType(typeof(void), 200)]
        [HttpPut]
        [Route("/api/notification/{notificationId}")]
        public async Task<IActionResult> UpdateNotificationAsync(string notificationId, [FromBody] dynamic notification)
        {
            var userId = GetCurrentSaasUserId();
            var request = new UpdateNotificationRequest(userId, notificationId, notification);
            await _notificationService.UpdateNotificationAsync(request);
            return Ok();
        }

        [ProducesResponseType(typeof(NotificationResponse), 200)]
        [HttpGet]
        [Route("/api/notification/{notificationId}")]
        public async Task<IActionResult> GetNotificationByIdAsync(string notificationId)
        {
            var userId = GetCurrentSaasUserId();
            var request = new GetNotificationRequest(userId, notificationId);
            var res = await _notificationService.GetNotificationByIdAsync(request);
            return Json(res);
        }
    }
}