// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.ChannelMember.TransportModels;

namespace Softeq.NetKit.Chat.Domain.ChannelMember
{
    public interface IChannelMemberService
    {
        Task<IEnumerable<ChannelMemberResponse>> GetChannelMembersAsync(ChannelRequest request);
        Task<ChannelMemberResponse> GetChannelMemberAsync(GetChannelMemberRequest request);
    }
}