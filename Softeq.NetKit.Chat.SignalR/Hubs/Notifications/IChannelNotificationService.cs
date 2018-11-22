// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.SignalR.Hubs.Notifications
{
    public interface IChannelNotificationService
    {
        Task OnJoinChannel(MemberSummary member, ChannelSummaryResponse channel);
        Task OnLeaveChannel(MemberSummary member, ChannelSummaryResponse channel);
        Task OnUpdateChannel(MemberSummary member, ChannelSummaryResponse channel);
        Task OnCloseChannel(MemberSummary member, ChannelSummaryResponse channel);
        Task OnAddChannel(MemberSummary member, ChannelSummaryResponse channel, string clientConnectionId);
    }
}