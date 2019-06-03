// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public interface IMessageNotificationService
    {
        Task OnAddMessage(MessageResponse message, string callerConnectionId = null);
        Task OnDeleteMessage(ChannelSummaryResponse channelSummary, MessageResponse message);
        Task OnUpdateMessage(MessageResponse message);
        Task OnAddMessageAttachment(Guid channelId);
        Task OnDeleteMessageAttachment(MessageResponse message);
        Task OnChangeLastReadMessage(List<Guid> notifyMemberIds, MessageResponse message);
    }
}