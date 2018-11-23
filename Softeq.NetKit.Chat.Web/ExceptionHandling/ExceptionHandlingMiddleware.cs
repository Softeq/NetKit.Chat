// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Serilog;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.Serilog.Extension;

namespace Softeq.NetKit.Chat.Web.ExceptionHandling
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IHostingEnvironment env)
        {
            try
            {
                await _next(context);
            }
            catch (NetKitChatException ex)
            {
                await HandleExceptionAsync(context, env, ex, StatusCodes.Status403Forbidden, ex.ErrorCode);
            }
            // TODO: split exception handling by StatusCodes
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, env, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, IHostingEnvironment env, Exception ex, int statusCode = StatusCodes.Status500InternalServerError, NetKitChatErrorCode? errorCode = null)
        {
            _logger.Event("UnhandledExceptionCaughtByMiddleware")
                   .With.Exception(ex)
                   .Message("Exception was caught by status code middleware. Status code = {StatusCode}; error code = {ErrorCode}", statusCode, errorCode)
                   .AsError();

            if (context.Response.HasStarted)
            {
                _logger.Event("UnableToModifyResponse")
                       .With.Exception(ex)
                       .Message("The response has already started, the http status code middleware will not be executed.")
                       .AsError();
                throw ex;
            }

            dynamic errorMessageObject = new ExpandoObject();
            errorMessageObject.Message = ex.Message;

            if (errorCode.HasValue)
            {
                errorMessageObject.ErrorCode = errorCode.Value;
            }

            if (!env.IsStaging() && !env.IsProduction())
            {
                errorMessageObject.StackTrace = ex.StackTrace;
            }

            var message = Newtonsoft.Json.JsonConvert.SerializeObject((object)errorMessageObject);

            context.Response.Clear();

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(message);
        }
    }
}