// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Softeq.CloudStorage.Extension;
using Softeq.NetKit.Chat.Domain.Exceptions.ErrorHandling;
using Softeq.NetKit.Chat.Domain.Services.Configuration;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/file")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    [ProducesResponseType(typeof(List<ErrorDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    public class FileController : BaseApiController
    {
        private const int TemporaryStorageAccessTokenExpirationTimeMinutes = 20;

        private readonly CloudStorageConfiguration _storageConfiguration;
        private readonly IContentStorage _contentStorage;

        public FileController(ILogger logger, CloudStorageConfiguration storageConfiguration, IContentStorage contentStorage)
            : base(logger)
        {
            Ensure.That(storageConfiguration).IsNotNull();
            Ensure.That(contentStorage).IsNotNull();

            _storageConfiguration = storageConfiguration;
            _contentStorage = contentStorage;
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [Route("get-access-token")]
        public async Task<IActionResult> GetTemporaryStorageAccessTokenAsync()
        {
            var accessToken = await _contentStorage.GetContainerSasTokenAsync(_storageConfiguration.TempContainerName, TemporaryStorageAccessTokenExpirationTimeMinutes);
            return Ok(accessToken);
        }
    }
}