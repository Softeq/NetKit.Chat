// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMessage;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public interface IDirectMessageNotificationService
    {
        Task OnAddMessage(DirectMessageResponse message, Guid memberId);
        Task OnDeleteMessage(DirectMessageResponse message, Guid memberId);
        Task OnUpdateMessage(DirectMessageResponse message, Guid memberId);
    }
}
