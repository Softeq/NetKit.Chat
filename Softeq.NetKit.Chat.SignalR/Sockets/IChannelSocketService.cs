// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;

namespace Softeq.NetKit.Chat.SignalR.Sockets
{
    public interface IChannelSocketService
    {
        Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest createChannelRequest);
        Task<ChannelSummaryResponse> UpdateChannelAsync(UpdateChannelRequest request);
        Task CloseChannelAsync(ChannelRequest request);
        Task JoinToChannelAsync(JoinToChannelRequest request);
        Task LeaveChannelAsync(ChannelRequest request);
        Task<ChannelResponse> InviteMemberAsync(InviteMemberRequest request);
        Task<ChannelResponse> InviteMultipleMembersAsync(InviteMembersRequest request);
        Task MuteChannelAsync(ChannelRequest request);
        Task PinChannelAsync(ChannelRequest request);
    }
}