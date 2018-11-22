// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Exceptions.ErrorHandling;

namespace Softeq.NetKit.Chat.Web.ExceptionHandling
{
    public class ExceptionHandler
    {
        protected ExceptionHandler()
        {
        }

        public static async Task Handle(HttpContext context, ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger(typeof(GlobalExceptionFilter));
            context.Response.ContentType = "application/json";
            var ex = context.Features.Get<IExceptionHandlerFeature>();
            if (ex != null)
            {
                var error = ex.Error;
                string json = null;
                if (error is ServiceException serviceException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    logger.LogError(error, $"ServiceException: {error.StackTrace}");
                    logger.LogError(error, $"Inner Excetion: {error.InnerException}");
                    json = JsonConvert.SerializeObject(serviceException.Errors);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    logger.LogError(error, $"Exception: {error.StackTrace}");
                    logger.LogError(error, $"Inner Excetion: {error.InnerException}");
                    var er = new List<ErrorDto>
                    {
                        new ErrorDto(ErrorCode.UnknownError, "Something went wrong during your request.")
                    };
                    json = JsonConvert.SerializeObject(er);
                }
                await context.Response.WriteAsync(json).ConfigureAwait(false);
            }
        }
    }
}
