// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Mappers;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.ChannelMember;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class ChannelMemberService : BaseService, IChannelMemberService
    {
        public ChannelMemberService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public async Task<IReadOnlyCollection<ChannelMemberResponse>> GetChannelMembersAsync(Guid channelId)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(channelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get channel members. Channel {nameof(channelId)}:{channelId} not found.");
            }

            var channelMembers = await UnitOfWork.ChannelMemberRepository.GetChannelMembersAsync(channelId);
            return channelMembers.Select(x => x.ToChannelMemberResponse()).ToList().AsReadOnly();
        }
    }
}