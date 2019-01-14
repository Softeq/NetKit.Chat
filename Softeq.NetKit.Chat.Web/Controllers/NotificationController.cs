using System.Threading.Tasks;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Chat.Notifications.PushNotifications;
using Softeq.NetKit.Chat.Notifications.PushNotifications.Model;
using Softeq.NetKit.Chat.Notifications.Services;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Request;
using Softeq.NetKit.Chat.Notifications.TransportModels.Notification.Response;
using Softeq.NetKit.Chat.Notifications.TransportModels.PushNotification.Request;
using Softeq.NetKit.Chat.Notifications.TransportModels.Shared.Request;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/notification")]
    [ApiVersion("1.0")]
    //[Authorize(Roles = "Admin, User")]
    public class NotificationController : BaseApiController
    {
        private PushNotificationService service;
        public NotificationController(ILogger logger, IConfiguration config) : base(logger)
        {
            service = new PushNotificationService(new NotificationHub(config));
        }

        [HttpGet]
        [Route("/api/register/notification")]
        [ProducesResponseType(typeof(NotificationResponse), StatusCodes.Status200OK)]
       // [Route("{deviceId:string}/register")]
        public async Task<IActionResult> RegisterDevice(string deviceId)
        {
            deviceId = "0963509501679c42";
            string saasUserId = "0C004B56-BB5B-44E1-93B3-FC413FCD5E81";
            await service.CreateOrUpdatePushSubscriptionAsync(new CreatePushTokenRequest(saasUserId, DevicePlatform.Android, deviceId));

            var push = new PushNotificationModel()
            {
                NotificationType = 1,
                Body = "alarma!!!!",
                Badge = 0,
                RecipientIds = new string[] {saasUserId},
                Title = "castom title"
            };
            await service.SendToSingleAsync(saasUserId, push);
            return Ok(new NotificationResponse(NotificationType.Message));
        }
    }
}