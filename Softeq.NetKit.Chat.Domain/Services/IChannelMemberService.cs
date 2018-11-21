// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.ChannelMember;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.ChannelMember;

namespace Softeq.NetKit.Chat.Domain.Services
{
    public interface IChannelMemberService
    {
        Task<IReadOnlyCollection<ChannelMemberResponse>> GetChannelMembersAsync(ChannelRequest request);
        Task<ChannelMemberResponse> GetChannelMemberAsync(GetChannelMemberRequest request);
    }
}