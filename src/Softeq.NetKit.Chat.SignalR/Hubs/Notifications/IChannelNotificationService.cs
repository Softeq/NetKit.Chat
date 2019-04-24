// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public interface IChannelNotificationService
    {
        Task OnJoinChannel(MemberSummaryResponse member, ChannelSummaryResponse channel);
        Task OnJoinDirectChannel(MemberSummaryResponse member, ChannelSummaryResponse channel);
        Task OnLeaveChannel(MemberSummaryResponse member, Guid channelId);
        Task OnDeletedFromChannel(MemberSummaryResponse member, Guid channelId);
        Task OnUpdateChannel(ChannelSummaryResponse channel);
        Task OnCloseChannel(ChannelSummaryResponse channel);
        Task OnAddChannel(ChannelSummaryResponse channel);
    }
}
