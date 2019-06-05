﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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