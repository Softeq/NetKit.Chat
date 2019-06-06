// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using Softeq.NetKit.Chat.Client.SDK.Enums;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Member;
using Softeq.NetKit.Chat.Client.SDK.Models.SignalRModels.Client;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;

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
                throw new NetKitChatNotFoundException($"Unable to get member by {nameof(saasUserId)}. Member {nameof(saasUserId)}:{saasUserId} is not found.");
            }

            return DomainModelsMapper.MapToMemberSummaryResponse(member);
        }

        public async Task<MemberSummaryResponse> GetMemberByIdAsync(Guid memberId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(memberId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member by {nameof(memberId)}. Member {nameof(memberId)}:{memberId} is not found.");
            }

            return DomainModelsMapper.MapToMemberSummaryResponse(member);
        }

        public async Task<IReadOnlyCollection<MemberSummaryResponse>> GetChannelMembersAsync(Guid channelId)
        {
            var isChannelExists = await UnitOfWork.ChannelRepository.IsChannelExistsAsync(channelId);
            if (!isChannelExists)
            {
                throw new NetKitChatNotFoundException($"Unable to get channel members. Channel {nameof(channelId)}:{channelId} is not found.");
            }

            var members = await UnitOfWork.MemberRepository.GetAllMembersByChannelIdAsync(channelId);
            return members.Select(member => DomainModelsMapper.MapToMemberSummaryResponse(member)).ToList().AsReadOnly();
        }

        public async Task<ChannelResponse> InviteMemberAsync(Guid memberId, Guid channelId)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelAsync(channelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to invite member. Channel {nameof(channelId)}:{channelId} is not found.");
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
                throw new NetKitChatNotFoundException($"Unable to invite member. Member {nameof(memberId)}:{memberId} is not found.");
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
                await UnitOfWork.ChannelRepository.IncrementChannelMembersCountAsync(channel.Id);
            });

            channel = await UnitOfWork.ChannelRepository.GetChannelAsync(channel.Id);

            return DomainModelsMapper.MapToChannelResponse(channel);
        }

        public async Task ActivateMemberAsync(string saasUserId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);

            if (member != null && !member.IsActive)
            {
                member.IsActive = true;
                await UnitOfWork.MemberRepository.ActivateMemberAsync(member);
            }
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

        public async Task<IReadOnlyCollection<DomainModels.Client>> GetMemberClientsAsync(Guid memberId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(memberId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member clients. Member {nameof(memberId)}:{memberId} is not found.");
            }

            var clients = await UnitOfWork.ClientRepository.GetMemberClientsAsync(member.Id);
            return clients.ToList().AsReadOnly();
        }

        public async Task UpdateMemberStatusAsync(string saasUserId, UserStatus status)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to update member status. Member {nameof(saasUserId)}:{saasUserId} is not found.");
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
            var members = await UnitOfWork.MemberRepository.GetPagedMembersExceptCurrentAsync(pageNumber, pageSize, nameFilter, currentUserSaasId);

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
                throw new NetKitChatNotFoundException($"Unable to update activity. Member {nameof(request.SaasUserId)}:{request.SaasUserId} is not found.");
            }
            member.Status = UserStatus.Online;
            member.LastActivity = _dateTimeProvider.GetUtcNow();
            await UnitOfWork.MemberRepository.UpdateMemberAsync(member);

            var client = await UnitOfWork.ClientRepository.GetClientWithMemberAsync(request.ConnectionId);
            if (client == null)
            {
                throw new NetKitChatNotFoundException($"Unable to update activity. Client {nameof(request.ConnectionId)}:{request.ConnectionId} is not found.");
            }
            client.UserAgent = request.UserAgent;
            client.LastActivity = member.LastActivity;
            client.LastClientActivity = _dateTimeProvider.GetUtcNow();
            await UnitOfWork.ClientRepository.UpdateClientAsync(client);
        }
    }
}
