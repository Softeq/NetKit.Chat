// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Security.Claims;
using EnsureThat;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/BaseApi")]
    public class BaseApiController : Controller
    {
        protected readonly ILogger Logger;

        public BaseApiController(ILogger logger)
        {
            Ensure.That(logger).IsNotNull();

            Logger = logger;
        }

        protected string GetCurrentSaasUserId()
        {
            return User.FindFirstValue(JwtClaimTypes.Subject);
        }
    }
}