// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.ChannelMember;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    public interface IChannelMemberService
    {
        Task<IReadOnlyCollection<ChannelMemberResponse>> GetChannelMembersAsync(Guid channelId);
        Task<IList<string>> GetSaasUserIdsWithDisabledChannelNotificationsAsync(Guid channelId);
    }
}