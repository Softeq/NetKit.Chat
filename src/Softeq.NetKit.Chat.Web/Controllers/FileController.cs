// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Chat.Data.Cloud.DataProviders;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/file")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    public class FileController : BaseApiController
    {
        private const int TemporaryStorageAccessTokenExpirationTimeMinutes = 20;

        private readonly ICloudTokenProvider _cloudTokenProvider;

        public FileController(ICloudTokenProvider cloudTokenProvider)
        {
            Ensure.That(cloudTokenProvider).IsNotNull();

            _cloudTokenProvider = cloudTokenProvider;
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [Route("get-access-token")]
        public async Task<IActionResult> GetTemporaryStorageAccessTokenAsync()
        {
            var accessToken = await _cloudTokenProvider.GetTemporaryStorageAccessTokenAsync(TemporaryStorageAccessTokenExpirationTimeMinutes);
            return Ok(accessToken);
        }
    }
}