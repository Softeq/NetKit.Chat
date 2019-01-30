// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public interface IDirectMessageNotificationService
    {
        Task OnAddMessage();
        Task OnDeleteMessage();
        Task OnUpdateMessage();
    }
}
