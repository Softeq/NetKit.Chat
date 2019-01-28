// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.DirectMembers;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.DirectMembers;
using System;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class DirectMessagesService : BaseService, IDirectMessageService
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IDirectChannelRepository _directChannelRepository;

        public DirectMessagesService(
            IUnitOfWork unitOfWork,
            IDomainModelsMapper domainModelsMapper,
            IDateTimeProvider dateTimeProvider)
            : base(unitOfWork, domainModelsMapper)
        {
            Ensure.That(dateTimeProvider).IsNotNull();

            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<DirectChannelResponse> CreateDirectMembers(CreateDirectMembersRequest request)
        {
            var firstDirectMember = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (firstDirectMember == null)
            {
                throw new NetKitChatNotFoundException($"Unable to create direct members. Member { nameof(request.SaasUserId) }:{ request.SaasUserId} is not found.");
            }

            var secondDirectMember = await UnitOfWork.MemberRepository.GetMemberByIdAsync(request.MemberId);
            if (secondDirectMember == null)
            {
                throw new NetKitChatNotFoundException($"Unable to create direct members. Member { nameof(request.MemberId) }:{ request.MemberId} is not found.");
            }

            await UnitOfWork.DirectChannelRepository.CreateDirectChannel(request.DirectMembersId, request.OwnerId, request.MemberId);

            return DomainModelsMapper.MapToDirectMembersResponse(request.DirectMembersId, firstDirectMember, secondDirectMember);
        }

        public async Task<DirectChannelResponse> GetDirectMembersById(Guid id)
        {
            var directMembers = await UnitOfWork.DirectChannelRepository.GetDirectChannelById(id);
            if (directMembers == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get direct members. Chat with {nameof(id)}:{id} is not found.");
            }

            var owner = await UnitOfWork.MemberRepository.GetMemberByIdAsync(directMembers.OwnerId);
            if (owner == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member {nameof(directMembers.OwnerId)}:{directMembers.OwnerId} is not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(directMembers.MemberId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member {nameof(directMembers.MemberId)}:{directMembers.MemberId} is not found.");
            }

            return DomainModelsMapper.MapToDirectMembersResponse(directMembers.Id, owner, member);
        }
    }
}
