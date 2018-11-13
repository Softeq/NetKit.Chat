// Developed by Softeq Development Corporation
// http://www.softeq.com
 
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Softeq.CloudStorage.Extension;
using Softeq.NetKit.Chat.Domain.Services.App.Configuration;
using Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/file")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    [ProducesResponseType(typeof(List<ErrorDto>), 400)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [ProducesResponseType(typeof(ErrorDto), 500)]
    public class FileController : BaseApiController
    {
        private readonly CloudStorageConfiguration _storageConfiguration;
        private readonly IContentStorage _contentStorage;

        public FileController(ILogger logger, CloudStorageConfiguration storageConfiguration, IContentStorage contentStorage) : base(logger)
        {
            _storageConfiguration = storageConfiguration;
            _contentStorage = contentStorage;
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        [Route("get-access-token")]
        public async Task<IActionResult> GetChannelInfoByIdAsync()
        {
            var accessToken = await _contentStorage.GetContainerSasTokenAsync(_storageConfiguration.TempContainerName, 20);
            return Ok(accessToken);
        }
    }
}