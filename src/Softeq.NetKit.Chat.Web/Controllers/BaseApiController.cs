// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Security.Claims;
using FluentValidation;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/BaseApi")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class BaseApiController : Controller
    {
        protected readonly IServiceProvider _serviceProvider;

        public BaseApiController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected string GetCurrentSaasUserId()
        {
            return User.FindFirstValue(JwtClaimTypes.Subject);
        }

        protected string GetCurrentUserName()
        {
            return User.FindFirstValue(JwtClaimTypes.Name);
        }

        protected string GetCurrentUserEmail()
        {
            return User.FindFirstValue(JwtClaimTypes.Email);
        }
    }
}