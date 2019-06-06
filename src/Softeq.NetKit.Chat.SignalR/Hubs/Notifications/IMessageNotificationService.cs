// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.DomainModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Message;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public interface IMessageNotificationService
    {
        Task OnAddMessage(MessageResponse message, RecipientType recipientType, string callerConnectionId = null);
        Task OnDeleteMessage(MessageResponse message);
        Task OnUpdateMessage(MessageResponse message);
        Task OnAddMessageAttachment(Guid channelId);
        Task OnDeleteMessageAttachment(MessageResponse message);
        Task OnChangeLastReadMessage(List<Guid> notifyMemberIds, Guid channelId);
    }
}