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

        [HttpGet]
        [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClientAsync(string connectionId)
        {
            var getClientRequest = new GetClientRequest(connectionId);
            var result = await _clientService.GetClientAsync(getClientRequest);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddClientAsync(string connectionId)
        {
            var addClientRequest = new AddClientRequest(GetCurrentSaasUserId(), GetCurrentUserName(), connectionId, null, GetCurrentUserEmail());
            var result = await _clientService.AddClientAsync(addClientRequest);
            return Ok(result);
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