// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.SignalR.Sockets
{
    public interface IDirectMessageSocketService
    {
        Task AddMessage();
        Task DeleteMessage();
        Task UpdateMessage();
    }
}
