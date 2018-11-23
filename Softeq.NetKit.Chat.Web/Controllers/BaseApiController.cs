// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Security.Claims;
using EnsureThat;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Softeq.NetKit.Chat.Domain.Exceptions_OLD.ErrorHandling;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/BaseApi")]
    [ProducesResponseType(typeof(List<ErrorDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
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