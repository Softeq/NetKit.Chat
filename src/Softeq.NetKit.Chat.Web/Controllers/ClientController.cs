// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Model = Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Request.Client;

namespace Softeq.NetKit.Chat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/client/{connectionId}")]
    [Authorize(Roles = "Admin, User")]
    [ApiVersion("1.0")]
    public class ClientController : BaseApiController
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            Ensure.That(clientService).IsNotNull();

            _clientService = clientService;
        }

        [HttpDelete]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteClientAsync(string connectionId)
        {
            await _clientService.DeleteClientAsync(new DeleteClientRequest(connectionId));
            return Ok();
        }
    }
}