// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.SystemMessage;
using System;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public interface IChannelNotificationService
    {
        Task OnJoinChannel(MemberSummary member, ChannelSummaryResponse channel);
        Task OnLeaveChannel(MemberSummary member, Guid channelId);
        Task OnDeletedFromChannel(MemberSummary member, Guid channelId);
        Task OnUpdateChannel(ChannelSummaryResponse channel);
        Task OnCloseChannel(ChannelSummaryResponse channel);
        Task OnAddChannel(ChannelSummaryResponse channel);
        Task OnAddSystemMessage(SystemMessageResponse response);
    }
}