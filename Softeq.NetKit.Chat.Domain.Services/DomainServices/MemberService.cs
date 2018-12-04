﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using EnsureThat;
using Softeq.NetKit.Chat.Data.Persistent;
using Softeq.NetKit.Chat.Domain.DomainModels;
using Softeq.NetKit.Chat.Domain.Exceptions;
using Softeq.NetKit.Chat.Domain.Services.Configuration;
using Softeq.NetKit.Chat.Domain.Services.Mappers;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;

namespace Softeq.NetKit.Chat.Domain.Services.DomainServices
{
    internal class MemberService : BaseService, IMemberService
    {
        private readonly CloudStorageConfiguration _configuration;

        public MemberService(IUnitOfWork unitOfWork, CloudStorageConfiguration configuration)
            : base(unitOfWork)
        {
            Ensure.That(configuration).IsNotNull();

            _configuration = configuration;
        }

        //TODO: Add Unit Tests
        public async Task<MemberSummary> GetMemberBySaasUserIdAsync(string saasUserId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member by {nameof(saasUserId)}. Member {nameof(saasUserId)}:{saasUserId} not found.");
            }

            return member.ToMemberSummary(_configuration);
        }

        public async Task<MemberSummary> GetMemberByIdAsync(Guid memberId)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(memberId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to get member by {nameof(memberId)}. Member {nameof(memberId)}:{memberId} not found.");
            }

            return member.ToMemberSummary(_configuration);
        }

        public async Task<IReadOnlyCollection<MemberSummary>> GetChannelMembersAsync(Guid channelId)
        {
            var members = await UnitOfWork.MemberRepository.GetAllMembersByChannelIdAsync(channelId);
            return members.Select(x => x.ToMemberSummary(_configuration)).ToList().AsReadOnly();
        }

        public async Task<ChannelResponse> InviteMemberAsync(InviteMemberRequest request)
        {
            var channel = await UnitOfWork.ChannelRepository.GetChannelByIdAsync(request.ChannelId);
            if (channel == null)
            {
                throw new NetKitChatNotFoundException($"Unable to invite member. Channel {nameof(request.ChannelId)}:{request.ChannelId} not found.");
            }

            var member = await UnitOfWork.MemberRepository.GetMemberByIdAsync(request.MemberId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to invite member. Member {nameof(request.MemberId)}:{request.MemberId} not found.");
            }

            var channelMembers = await GetChannelMembersAsync(channel.Id);
            if (channelMembers.Any(x => x.Id == request.MemberId))
            {
                throw new NetKitChatInvalidOperationException($"Unable to invite member. Member {nameof(request.MemberId)}:{request.MemberId} already joined channel {nameof(request.ChannelId)}:{request.ChannelId}.");
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

        public async Task<ClientResponse> GetOrAddClientAsync(AddClientRequest request)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                var newMember = new Member
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
            else
            {
                await UpdateMemberStatusAsync(new UpdateMemberStatusRequest(member.SaasUserId, UserStatus.Active));
            }

            member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);         
           
            var client = await UnitOfWork.ClientRepository.GetClientByConnectionIdAsync(request.ConnectionId);
            if (client != null)
            {
                return client.ToClientResponse(member.SaasUserId);
            }

            client = new Client
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
            if (client == null)
            {
                throw new NetKitChatNotFoundException($"Unable to delete client. Client {nameof(request.ClientConnectionId)}:{request.ClientConnectionId} not found.");
            }

            await UnitOfWork.ClientRepository.DeleteClientAsync(client.Id);

            var clients = await GetMemberClientsAsync(client.MemberId);
            if (!clients.Any())
            {
                await UpdateMemberStatusAsync(new UpdateMemberStatusRequest(client.Member.SaasUserId, UserStatus.Offline));
            }
        }

        // TODO:Add unit test
        public async Task<IReadOnlyCollection<Client>> GetMemberClientsAsync(Guid memberId)
        {
            var clients = await UnitOfWork.ClientRepository.GetMemberClientsAsync(memberId);
            return clients.ToList().AsReadOnly();
        }

        public async Task<MemberSummary> AddMemberAsync(string saasUserId, string email)
        {
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(saasUserId);
            if (member != null)
            {
                // TODO [az]: Should we throw exception here?
                return member.ToMemberSummary(_configuration);
            }

            var newMember = new Member
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
            var member = await UnitOfWork.MemberRepository.GetMemberBySaasUserIdAsync(request.SaasUserId);
            if (member == null)
            {
                throw new NetKitChatNotFoundException($"Unable to update member status. Member {nameof(request.SaasUserId)}:{request.SaasUserId} not found.");
            }

            member.Status = request.UserStatus;
            member.LastActivity = DateTimeOffset.Now;
            await UnitOfWork.MemberRepository.UpdateMemberAsync(member);
        }

        public async Task<IReadOnlyCollection<ClientResponse>> GetClientsByMemberIds(List<Guid> memberIds)
        {
            var clients = await UnitOfWork.ClientRepository.GetClientsByMemberIdsAsync(memberIds);
            return clients.Select(x => x.ToClientResponse(x.Member.SaasUserId)).ToList().AsReadOnly();
        }

        public async Task<PagedMembersResponse> GetPagedMembersAsync(int pageNumber, int pageSize, string nameFilter)
        {
            var members = await UnitOfWork.MemberRepository.GetPagedMembersAsync(pageNumber, pageSize, nameFilter);

            var response = new PagedMembersResponse
            {
                Entities = members.Entities.Select(x => x.ToMemberSummary(_configuration)),
                TotalRows = members.TotalRows,
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
                Entities = members.Entities.Select(x => x.ToMemberSummary(_configuration)),
                TotalRows = members.TotalRows,
                PageNumber = members.PageNumber,
                PageSize = members.PageSize
            };

            return response;
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