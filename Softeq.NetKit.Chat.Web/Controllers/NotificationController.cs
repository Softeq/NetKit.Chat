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
       
        public NotificationController(ILogger logger) : base(logger)
        {
           
        }
    }
}