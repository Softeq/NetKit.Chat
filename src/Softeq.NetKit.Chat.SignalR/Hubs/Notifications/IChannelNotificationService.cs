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
        Task OnUpdateChannelPersonalized(ChannelSummaryResponse channel, Guid memberId, string currentConnectionId = "");
        Task OnJoinChannelPersonalized(ChannelSummaryResponse channel, Guid memberId, string currentConnectionId = "");
        Task OnJoinChannel(ChannelSummaryResponse channel, Guid memberId);
        Task OnLeaveChannel(MemberSummaryResponse member, Guid channelId);
        Task OnUpdateChannel(ChannelSummaryResponse channel);
        Task OnCloseChannel(Guid channelId);
    }
}
