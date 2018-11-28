// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public interface IMessageNotificationService
    {
        Task OnAddMessage(string saasUserId, MessageResponse message, string clientConnectionId);
        Task OnDeleteMessage(string saasUserId, MessageResponse message);
        Task OnUpdateMessage(string saasUserId, MessageResponse message);
        Task OnAddMessageAttachment(string saasUserId, Guid channelId);
        Task OnDeleteMessageAttachment(string saasUserId, MessageResponse message);
        Task OnChangeLastReadMessage(List<Guid> notifyMemberIds, MessageResponse message);
    }
}