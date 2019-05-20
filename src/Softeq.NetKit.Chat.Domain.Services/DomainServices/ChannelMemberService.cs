// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.ChannelMember;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class ChannelMemberService : BaseService, IChannelMemberService
    {
        public ChannelMemberService(IUnitOfWork unitOfWork, IDomainModelsMapper domainModelsMapper)
            : base(unitOfWork, domainModelsMapper)
        {
        }

        public async Task<IReadOnlyCollection<ChannelMemberResponse>> GetChannelMembersAsync(Guid channelId)
        {
            var isChannelExists = await UnitOfWork.ChannelRepository.IsChannelExistsAsync(channelId);
            if (!isChannelExists)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableGetMembersCauseNoChannel, $"{ nameof(channelId) }:{ channelId}");
            }

            var channelMembers = await UnitOfWork.ChannelMemberRepository.GetChannelMembersAsync(channelId);
            return channelMembers.Select(channelMember => DomainModelsMapper.MapToChannelMemberResponse(channelMember)).ToList().AsReadOnly();
        }

        public async Task<IList<string>> GetSaasUserIdsWithDisabledChannelNotificationsAsync(Guid channelId)
        {
            return await UnitOfWork.ChannelMemberRepository.GetSaasUserIdsWithDisabledChannelNotificationsAsync(channelId);
        }
    }
}