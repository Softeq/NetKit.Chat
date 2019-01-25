// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public interface IDirectMessageNotificationService
    {
        Task OnCreateDirectMembers(DirectMembersResponse request, string connectionId);
    }
}
