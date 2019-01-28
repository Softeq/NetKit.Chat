// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public interface IDirectMessageNotificationService
    {
        Task OnCreateDirectMembers(DirectChannelResponse request, string connectionId);
    }
}
