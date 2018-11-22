// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Exceptions.ErrorHandling;
using Softeq.Serilog.Extension;

namespace Softeq.NetKit.Chat.Web.ExceptionHandling
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public GlobalExceptionFilter(ILogger logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            var username = context.HttpContext.User.FindFirstValue(JwtClaimTypes.Name);
            var url = context.HttpContext.Request.Path.Value;
            if (context.Exception is ServiceException)
            {
                _logger.Event("ServiceException").With.Message("Requester UserName: {username}, Url: {url}", username, url).Exception(context.Exception).AsError();
                var serviceException = (ServiceException)(context.Exception);
                var response = new ObjectResult(serviceException.Errors) { StatusCode = 400 };
                context.Result = response;
            }
            else
            {
                _logger.Event("UnknownException").With.Message("Requester UserName: {username}, Url: {url}", username, url).Exception(context.Exception).AsError();
                var response = new ObjectResult(new List<ErrorDto>
                    {
                        new ErrorDto(ErrorCode.UnknownError, "Something went wrong during your request.")
                    })
                    { StatusCode = 500 };
                context.Result = response;
            }
        }
    }
}
