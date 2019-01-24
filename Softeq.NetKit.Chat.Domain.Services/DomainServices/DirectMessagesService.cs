// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Data.Persistent.Repositories;
using Softeq.NetKit.Chat.Domain.DomainModels;
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
        private readonly IDirectMemberRepository _directMemberRepository;

        public DirectMessagesService(
            IUnitOfWork unitOfWork,
            IDomainModelsMapper domainModelsMapper,
            IDateTimeProvider dateTimeProvider)
            : base(unitOfWork, domainModelsMapper)
        {
            Ensure.That(dateTimeProvider).IsNotNull();

            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<CreateDirectMembersResponse> CreateDirectMembers(CreateDirectMembersRequest request)
        {
            var firstDirectMember = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (firstDirectMember == null)
            {
                throw new NetKitChatNotFoundException($"Unable to create direct members.Member { nameof(request.SaasUserId) }:{ request.SaasUserId} is not found.");
            }

            var secondDirectMember = await UnitOfWork.MemberRepository.GetMemberByIdAsync(request.SecondMemberId);
            if (secondDirectMember == null)
            {
                throw new NetKitChatNotFoundException($"Unable to create direct members.Member { nameof(request.SaasUserId) }:{ request.SaasUserId} is not found.");
            }

            await UnitOfWork.DirectMemberRepository.CreateDirectMembers(request.DirectId, request.FirstMemberId, request.SecondMemberId);

            return DomainModelsMapper.MapToDirectMembersResponse(request.DirectId, firstDirectMember, secondDirectMember);
        }

        public Task<DirectMembers> GetDirectMembersById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
