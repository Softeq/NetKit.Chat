// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.TransportModels.Models.CommonModels.Response.Member;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public interface IChannelNotificationService
    {
        Task OnJoinChannel(ChannelSummaryResponse channel);
        Task OnJoinChannel(ChannelSummaryResponse channel, Guid memberId);
        Task OnLeaveChannel(MemberSummaryResponse member, Guid channelId);
        Task OnDeletedFromChannel(MemberSummaryResponse member, Guid channelId);
        Task OnUpdateChannel(ChannelSummaryResponse channel);
        Task OnCloseChannel(Guid channelId);
    }
}
