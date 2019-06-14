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

        protected void ValidateAndThrow<T>(T model)
        {
            var validator = (IValidator<T>)_serviceProvider.GetService(typeof(IValidator<T>));
            if (validator == null)
            {
                throw new InvalidOperationException($"Could not resolve validator for specified model. Model type: {typeof(T)}");
            }

            var result = validator.Validate(model);
            if (!result.IsValid)
            {
                var resultExceptionMessage = string.Empty;

                foreach (var error in result.Errors)
                {
                    resultExceptionMessage += $"{error.ErrorMessage}: {error.PropertyName}";
                }

                throw new ValidationException($"Validation error in {model}. {resultExceptionMessage}");
            }
        }
    }
}