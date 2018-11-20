// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using EnsureThat;
using Softeq.NetKit.Chat.Data.Repositories;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.ChannelMember;
using Softeq.NetKit.Chat.Domain.Client.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Client.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Services.App.Configuration;
using Softeq.NetKit.Chat.Domain.Services.Channel;
using Softeq.NetKit.Chat.Domain.Services.Client;
using Softeq.NetKit.Chat.Domain.Services.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Exceptions.ErrorHandling;

namespace Softeq.NetKit.Chat.Domain.Services.Member
{
    internal class MemberService : BaseService, IMemberService
    {
        private readonly CloudStorageConfiguration _configuration;

        public MemberService(IUnitOfWork unitOfWork, CloudStorageConfiguration configuration) : base(unitOfWork)
        {
            _configuration = configuration;
        }
        
        public async Task<ParticipantResponse> SurelyGetMemberBySaasUserIdAsync(string saasUserId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            Ensure.That(member).WithException(x => new ServiceException(new ErrorDto(ErrorCode.NotFound, "Member does not exist."))).IsNotNull();
            return member.ToParticipantResponse();
        }
        //TODO: Add Unit Tests
        public async Task<MemberSummary> GetMemberSummaryBySaasUserIdAsync(string saasUserId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            Ensure.That(member).WithException(x => new ServiceException(new ErrorDto(ErrorCode.NotFound, "Member does not exist."))).IsNotNull();
            return member.ToMemberSummary(_configuration);
        }

        public async Task<MemberSummary> GetMemberByIdAsync(Guid memberId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(memberId);
            Ensure.That(member).WithException(x => new ServiceException(new ErrorDto(ErrorCode.NotFound, "Member does not exist."))).IsNotNull();
            return member.ToMemberSummary(_configuration);
        }

        public async Task<ParticipantResponse> GetMemberAsync(UserRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            Ensure.That(member).WithException(x => new ServiceException(new ErrorDto(ErrorCode.NotFound, "Member does not exist."))).IsNotNull();
            return member.ToParticipantResponse();
        }

        public async Task<IReadOnlyCollection<MemberSummary>> GetChannelMembersAsync(Guid channelId)
        {
            var members = await UnitOfWork.MemberRepository.GetAllMembersByChannelIdAsync(channelId);
            return members.Select(x => x.ToMemberSummary(_configuration)).ToList().AsReadOnly();
        }

        public async Task<ChannelResponse> InviteMemberAsync(InviteMemberRequest request)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(request.ChannelId);
            Ensure.That(channel).WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Channel does not exist."))).IsNotNull();
            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(request.MemberId);
            Ensure.That(member).WithException(x => new ServiceException(new ErrorDto(ErrorCode.NotFound, "Member does not exist."))).IsNotNull();

            var channelMembers = await GetChannelMembersAsync(channel.Id);

            if (channelMembers.Any(x => x.Id == request.MemberId))
            {
                throw new ConflictException(new ErrorDto(ErrorCode.ConflictError, "User already exists in channel."));
            }

            var channelMember = new ChannelMembers
            {
                ChannelId = request.ChannelId,
                MemberId = request.MemberId,
                LastReadMessageId = null
            };

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
                await UnitOfWork.ChannelRepository.IncrementChannelMembersCount(channel.Id);

                transactionScope.Complete();
            }

            var newChannel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(channel.Id);

            return newChannel.ToChannelResponse(_configuration);
        }

        public async Task<IEnumerable<ParticipantResponse>> GetOnlineChannelMembersAsync(ChannelRequest request)
        {
            var members = await UnitOfWork.MemberRepository.GetOnlineMembersInChannelAsync(request.ChannelId);
            return members.Select(x => x.ToParticipantResponse());
        }

        // TODO:Add unit test
        public async Task<ClientResponse> GetOrAddClientAsync(AddClientRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                var newMember = new Domain.Member.Member
                {
                    Id = Guid.NewGuid(),
                    Role = UserRole.User,
                    IsAfk = false,
                    IsBanned = false,
                    Status = UserStatus.Active,
                    Name = request.UserName,
                    LastActivity = DateTimeOffset.UtcNow,
                    SaasUserId = request.SaasUserId
                };
                await UnitOfWork.MemberRepository.AddMemberAsync(newMember);
            }

            member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);

            var client = await UnitOfWork.ClientRepository.GetClientByConnectionIdAsync(request.ConnectionId);
            if (client != null)
            {
                return client.ToClientResponse(member.SaasUserId);
            }

            client = new Domain.Client.Client
            {
                Id = Guid.NewGuid(),
                MemberId = member.Id,
                ClientConnectionId = request.ConnectionId,
                LastActivity = member.LastActivity,
                LastClientActivity = DateTimeOffset.UtcNow,
                Name = request.UserName,
                UserAgent = request.UserAgent
            };

            await UnitOfWork.ClientRepository.AddClientAsync(client);
            return client.ToClientResponse(member.SaasUserId);
        }

        public async Task DeleteClientAsync(DeleteClientRequest request)
        {
            var client = await UnitOfWork.ClientRepository.GetClientByConnectionIdAsync(request.ClientConnectionId);
            Ensure.That(client).WithException(x => new NotFoundException(new ErrorDto(ErrorCode.NotFound, "Client does not exist.")));
            await UnitOfWork.ClientRepository.DeleteClientAsync(client.Id);
        }

        // TODO:Add unit test
        public async Task<IEnumerable<Domain.Client.Client>> GetMemberClientsAsync(Guid memberId)
        {
            var clients = await UnitOfWork.ClientRepository.GetMemberClientsAsync(memberId);
            return clients;
        }

        public async Task<MemberSummary> AddMemberAsync(string saasUserId, string email)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member != null)
            {
                return member.ToMemberSummary(_configuration);
            }

            var newMember = new Domain.Member.Member
            {
                Id = Guid.NewGuid(),
                Role = UserRole.User,
                IsAfk = false,
                IsBanned = false,
                Status = UserStatus.Active,
                SaasUserId = saasUserId,
                Email = email,
                LastActivity = DateTimeOffset.UtcNow,
                Name = email,
            };
            await UnitOfWork.MemberRepository.AddMemberAsync(newMember);

            return newMember.ToMemberSummary(_configuration);
        }

        public async Task UpdateMemberStatusAsync(UpdateMemberStatusRequest request)
        {
            var member = await SurelyGetMemberBySaasUserIdAsync(request.SaasUserId);
            member.Status = request.UserStatus;
            await UnitOfWork.MemberRepository.UpdateMemberAsync(member.ToMember());
        }

        public async Task<IEnumerable<ClientResponse>> GetClientsByMemberIds(List<Guid> memberIds)
        {
            var clients = await UnitOfWork.ClientRepository.GetClientsByMemberIdsAsync(memberIds);
            return clients.Select(x => x.ToClientResponse(x.Member.SaasUserId));
        }

        public async Task<IEnumerable<MemberSummary>> GetAllMembersAsync()
        {
            var members = await UnitOfWork.MemberRepository.GetAllMembersAsync();
            return members.Select(x => x.ToMemberSummary(_configuration));
        }

        public async Task UpdateActivityAsync(AddClientRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            member.Status = UserStatus.Active;
            member.LastActivity = DateTimeOffset.UtcNow;

            var client = await UnitOfWork.ClientRepository.GetClientByConnectionIdAsync(request.ConnectionId);
            client.UserAgent = request.UserAgent;
            client.LastActivity = member.LastActivity;
            client.LastClientActivity = DateTimeOffset.UtcNow;

            // Remove any Afk notes.
            if (member.IsAfk)
            {
                member.IsAfk = false;
            }

            await UnitOfWork.MemberRepository.UpdateMemberAsync(member);
            await UnitOfWork.ClientRepository.UpdateClientAsync(client);
        }
    }
}