// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectMembers;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers;

namespace Softeq.NetKit.Chat.SignalR.Sockets
{
    public interface IDirectMessageSocketService
    {
        Task<DirectMembersResponse> CreateDirectMembers(CreateDirectMembersRequest request, string connectionId);
    }
}
