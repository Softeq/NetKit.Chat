// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Message.TransportModels.Response;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public interface IMessageNotificationService
    {
        Task OnAddMessage(MemberSummary member, MessageResponse message, string clientConnectionId);
        Task OnDeleteMessage(MemberSummary member, MessageResponse message);
        Task OnUpdateMessage(MemberSummary member, MessageResponse message);
        Task OnAddMessageAttachment(MemberSummary member, MessageResponse message);
        Task OnDeleteMessageAttachment(MemberSummary member, MessageResponse message);
        Task OnChangeLastReadMessage(List<MemberSummary> members, MessageResponse message);
    }
}