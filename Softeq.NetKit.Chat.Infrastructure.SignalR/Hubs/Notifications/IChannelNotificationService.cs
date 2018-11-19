// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Response;

namespace Softeq.NetKit.Chat.Infrastructure.SignalR.Hubs.Notifications
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