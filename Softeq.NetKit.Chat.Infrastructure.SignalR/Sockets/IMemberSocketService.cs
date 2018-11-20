using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Response;

namespace Softeq.NetKit.Chat.Infrastructure.SignalR.Sockets
{
    public interface IMemberSocketService
    {
        Task<IReadOnlyCollection<MemberSummary>> GetChannelMembersAsync(Guid channelId);
    }
}