using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Data.Interfaces.Repository;
using Softeq.NetKit.Chat.Domain.Client.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Client.TransportModels.Response;

namespace Softeq.NetKit.Chat.Data.Interfaces.SocketConnection
{
    public interface IClientService
    {
        Task<ConnectionResponse> GetOrAddClientAsync(AddConnectionRequest request);
        Task DeleteClientAsync(DeleteConnectionRequest request);
        Task UpdateActivityAsync(AddConnectionRequest request);
    }
}