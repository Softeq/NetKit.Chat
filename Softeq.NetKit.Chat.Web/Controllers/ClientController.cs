// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/client")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    public class ClientController : BaseApiController
    {
        public ClientController(ILogger logger)
            : base(logger)
        {
            throw new NotImplementedException();
        }
    }
}