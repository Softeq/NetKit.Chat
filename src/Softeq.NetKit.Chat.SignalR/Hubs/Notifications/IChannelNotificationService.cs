// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Member;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public interface IChannelNotificationService
    {
        Task OnJoinChannel(ChannelSummaryResponse channel);
        Task OnJoinDirectChannel(MemberSummaryResponse member, ChannelSummaryResponse channel);
        Task OnLeaveChannel(MemberSummaryResponse member, Guid channelId);
        Task OnDeletedFromChannel(MemberSummaryResponse member, Guid channelId);
        Task OnUpdateChannel(ChannelSummaryResponse channel);
        Task OnCloseChannel(Guid channelId);
        Task OnAddChannel(ChannelSummaryResponse channel);
    }
}
