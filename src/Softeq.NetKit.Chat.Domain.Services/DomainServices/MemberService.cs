// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class MemberService : BaseService, IMemberService
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public MemberService(
            IUnitOfWork unitOfWork,
            IDomainModelsMapper domainModelsMapper,
            IDateTimeProvider dateTimeProvider)
            : base(unitOfWork, domainModelsMapper)
        {
            Ensure.That(dateTimeProvider).IsNotNull();

            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<MemberSummaryResponse> GetMemberBySaasUserIdAsync(string saasUserId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableGetMember, $"{nameof(saasUserId)}", $"{nameof(saasUserId)}:{saasUserId}");
            }

            return DomainModelsMapper.MapToMemberSummaryResponse(member);
        }

        public async Task<MemberSummaryResponse> GetMemberByIdAsync(Guid memberId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(memberId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableGetMember, $"{ nameof(memberId) }", $"{nameof(memberId)}:{memberId}");
            }

            return DomainModelsMapper.MapToMemberSummaryResponse(member);
        }

        public async Task<IReadOnlyCollection<MemberSummaryResponse>> GetChannelMembersAsync(Guid channelId)
        {
            var isChannelExists = await UnitOfWork.ChannelRepository.IsChannelExistsAsync(channelId);
            if (!isChannelExists)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableGetMembersCauseNoChannel, $"{nameof(channelId)}:{channelId}");
            }

            var members = await UnitOfWork.MemberRepository.GetAllMembersByChannelIdAsync(channelId);
            return members.Select(member => DomainModelsMapper.MapToMemberSummaryResponse(member)).ToList().AsReadOnly();
        }

        public async Task<ChannelResponse> InviteMemberAsync(Guid memberId, Guid channelId)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelAsync(channelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableInviteMemberCauseNoChannel, $"{nameof(channelId)}:{channelId}");
            }

            if (channel.Type == ChannelType.Direct)
            {
                throw new NetKitChatInvalidOperationException($"Unable to invite member to Direct Channel {nameof(channelId)}:{channelId}.");
            }

            if (channel.IsClosed)
            {
                throw new NetKitChatInvalidOperationException($"Unable to invite member. Channel {nameof(channelId)}:{channelId} is closed.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(memberId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableInviteMember, $"{nameof(memberId)}:{memberId}");
            }

            var isMemberExistsInChannel = await UnitOfWork.ChannelRepository.IsMemberExistsInChannelAsync(member.Id, channel.Id);
            if (isMemberExistsInChannel)
            {
                throw new NetKitChatInvalidOperationException($"Unable to invite member. Member {nameof(memberId)}:{memberId} already joined channel {nameof(channelId)}:{channelId}.");
            }

            var channelMember = new ChannelMember
            {
                ChannelId = channel.Id,
                MemberId = member.Id,
                LastReadMessageId = null
            };

            await UnitOfWork.ExecuteTransactionAsync(async () =>
            {
                await UnitOfWork.ChannelMemberRepository.AddChannelMemberAsync(channelMember);
                await UnitOfWork.ChannelRepository.IncrementChannelMembersCount(channel.Id);
            });

            channel = await UnitOfWork.ChannelRepository.GetChannelAsync(channel.Id);

            return DomainModelsMapper.MapToChannelResponse(channel);
        }

        public async Task ActivateMemberAsync(string saasUserId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member == null)
            {
                throw new NetKitChatInvalidOperationException($"Unable to activate member. Member {nameof(saasUserId)}:{saasUserId} is not found.");
            }

            member.IsActive = true;

            await UnitOfWork.MemberRepository.ActivateMemberAsync(member);
        }

        public async Task<MemberSummaryResponse> AddMemberAsync(string saasUserId, string email)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member != null)
            {
                throw new NetKitChatInvalidOperationException($"Unable to add member. Member {nameof(saasUserId)}:{saasUserId} already exists.");
            }

            var newMember = new DomainModels.Member
            {
                Id = Guid.NewGuid(),
                Role = UserRole.User,
                IsAfk = false,
                IsBanned = false,
                Status = UserStatus.Online,
                SaasUserId = saasUserId,
                Email = email,
                LastActivity = _dateTimeProvider.GetUtcNow(),
                Name = email
            };
            await UnitOfWork.MemberRepository.AddMemberAsync(newMember);

            return DomainModelsMapper.MapToMemberSummaryResponse(newMember);
        }

        public async Task<IReadOnlyCollection<Client>> GetMemberClientsAsync(Guid memberId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(memberId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableGetMemberClients, $"{nameof(memberId)}:{memberId}");
            }

            var clients = await UnitOfWork.ClientRepository.GetMemberClientsAsync(member.Id);
            return clients.ToList().AsReadOnly();
        }

        public async Task UpdateMemberStatusAsync(string saasUserId, UserStatus status)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableUpdateMemberStatus, $"{nameof(saasUserId)}:{saasUserId}");
            }

            member.Status = status;
            member.LastActivity = _dateTimeProvider.GetUtcNow();
            await UnitOfWork.MemberRepository.UpdateMemberAsync(member);
        }

        public async Task<IReadOnlyCollection<ClientResponse>> GetClientsByMemberIds(List<Guid> memberIds)
        {
            var clients = await UnitOfWork.ClientRepository.GetClientsWithMembersAsync(memberIds);
            return clients.Select(client => DomainModelsMapper.MapToClientResponse(client)).ToList().AsReadOnly();
        }

        public async Task<PagedMembersResponse> GetPagedMembersAsync(int pageNumber, int pageSize, string nameFilter, string currentUserSaasId)
        {
            var members = await UnitOfWork.MemberRepository.GetPagedMembersAsync(pageNumber, pageSize, nameFilter, currentUserSaasId);

            var response = new PagedMembersResponse
            {
                Results = members.Results.Select(member => DomainModelsMapper.MapToMemberSummaryResponse(member)),
                TotalNumberOfItems = members.TotalNumberOfItems,
                TotalNumberOfPages = members.TotalNumberOfPages,
                PageNumber = members.PageNumber,
                PageSize = members.PageSize
            };

            return response;
        }

        public async Task<PagedMembersResponse> GetPotentialChannelMembersAsync(Guid channelId, GetPotentialChannelMembersRequest request)
        {
            var members = await UnitOfWork.MemberRepository.GetPotentialChannelMembersAsync(channelId, request.PageNumber, request.PageSize, request.NameFilter);

            var response = new PagedMembersResponse
            {
                Results = members.Results.Select(member => DomainModelsMapper.MapToMemberSummaryResponse(member)),
                TotalNumberOfItems = members.TotalNumberOfItems,
                TotalNumberOfPages = members.TotalNumberOfPages,
                PageNumber = members.PageNumber,
                PageSize = members.PageSize
            };

            return response;
        }

        public async Task UpdateActivityAsync(UpdateMemberActivityRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableUpdateActivityCauseNoMember, $"{nameof(request.SaasUserId)}:{request.SaasUserId}");
            }
            member.Status = UserStatus.Online;
            member.LastActivity = _dateTimeProvider.GetUtcNow();
            member.IsAfk = false;
            await UnitOfWork.MemberRepository.UpdateMemberAsync(member);

            var client = await UnitOfWork.ClientRepository.GetClientWithMemberAsync(request.ConnectionId);
            if (client == null)
            {
                throw new NetKitChatNotFoundException(NetKitChatNotFoundErrorMessages.UnableUpdateActivityCauseNoClient, $"{nameof(request.ConnectionId)}:{request.ConnectionId}");
            }
            client.UserAgent = request.UserAgent;
            client.LastActivity = member.LastActivity;
            client.LastClientActivity = _dateTimeProvider.GetUtcNow();
            await UnitOfWork.ClientRepository.UpdateClientAsync(client);
        }
    }
}
