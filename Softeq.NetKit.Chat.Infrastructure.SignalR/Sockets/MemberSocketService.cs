// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Response;

namespace Softeq.NetKit.Chat.Infrastructure.SignalR.Sockets
{
    public class MemberSocketService : IMemberSocketService
    {
        private readonly IMemberService _memberService;

        public MemberSocketService(IMemberService memberService)
        {
            Ensure.That(memberService).IsNotNull();

            _memberService = memberService;
        }

        public Task<IReadOnlyCollection<MemberSummary>> GetChannelMembersAsync(Guid channelId)
        {
            return _memberService.GetChannelMembersAsync(channelId);
        }
    }
}