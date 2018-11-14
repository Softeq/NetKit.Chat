// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Softeq.NetKit.Chat.Common.Cache;
using Softeq.NetKit.Chat.Data.Interfaces.SocketConnection;
using Softeq.NetKit.Chat.Data.Interfaces.UnitOfWork;
using Softeq.NetKit.Chat.Data.Repositories;
using Softeq.NetKit.Chat.Domain.Channel;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Channel.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.ChannelMember;
using Softeq.NetKit.Chat.Domain.ChannelMember.TransportModels;
using Softeq.NetKit.Chat.Domain.Client;
using Softeq.NetKit.Chat.Domain.Client.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Client.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Member;
using Softeq.NetKit.Chat.Domain.Member.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.Message;
using Softeq.NetKit.Chat.Domain.Message.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.Message.TransportModels.Response;
using Softeq.NetKit.Chat.Infrastructure.SignalR.Sockets;
using Softeq.Serilog.Extension;
using Softeq.NetKit.Chat.Domain.Services.Client;


namespace Softeq.NetKit.Chat.Infrastructure.SignalR.Hubs
{
    public class ChatHub : Hub, IChannelNotificationHub, IMessageNotificationHub
    {
        private readonly IMemberService _memberService;
        private readonly IChannelService _channelService;
        private readonly IChannelMemberService _channelMemberService;
        private IClientService _clientService;
        private readonly ILogger _logger;

        private readonly ChannelSocketService _channelSocketService;
        private readonly MessageSocketService _messageSocketService;

        public ChatHub(
            IChannelService channelService,
            IMemberService memberService,
            IClientService clientService,
            IDistributedCacheClient redisClient,
            ILogger logger,
            IMessageService messageService, 
            IUnitOfWork unitOfWork,
            IChannelMemberService channelMemberService)
        {
            _channelService = channelService;
            _memberService = memberService;
            _channelMemberService = channelMemberService;
            _clientService = clientService;
            _logger = logger;
           
            _channelSocketService = new ChannelSocketService(_channelService, _logger, _memberService, this);
            _messageSocketService = new MessageSocketService(_channelService, _logger, _memberService, messageService, this);
        }

        #region Override

        public override async Task OnConnectedAsync()
        {
            _logger.Event(PropertyNames.EventId).With.Message($"SignalR Client OnConnectedAsync({Context.ConnectionId})", Context.ConnectionId).AsInformation();
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var deleteRequest = new DeleteConnectionRequest(Context.ConnectionId);
            _clientService.DeleteClientAsync(deleteRequest);
            _logger.Event(PropertyNames.EventId).With.Message($"SignalR Client OnDisconnectedAsync({Context.ConnectionId})", Context.ConnectionId).AsInformation();
            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        #region Client Hub Commands

        public async Task<ConnectionResponse> AddClientAsync()
        {
            return await CheckAccessTokenAndExecute(new TaskReference<ConnectionResponse>(async () =>
            {
                var addClientRequest = new AddConnectionRequest
                {
                    ConnectionId = Context.ConnectionId,
                    UserAgent = null,
                    UserName = Context.GetUserName(),
                    SaasUserId = Context.GetSaasUserId()
                };

                return await _clientService.GetOrAddClientAsync(addClientRequest);
            }));
        }

        public async Task DeleteClientAsync()
        {
            await CheckAccessTokenAndExecute(new TaskReference(async () =>
            {
                var deleteClientRequest = new DeleteConnectionRequest(Context.ConnectionId);
                deleteClientRequest.SaasUserId = Context.GetSaasUserId();
                await _clientService.DeleteClientAsync(deleteClientRequest);
            }));
        }

        #endregion

        #region Message Hub Commands

        public async Task<MessageResponse> AddMessageAsync(CreateMessageRequest request)
        {
            return await CheckAccessTokenAndExecute(new TaskReference<MessageResponse>(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                request.ClientConnectionId = Context.ConnectionId;
                var message = await _messageSocketService.AddMessageAsync(request);
                var user = await _memberService.GetMemberSummaryBySaasUserIdAsync(request.SaasUserId);
                var addClientRequest = new AddConnectionRequest()
                {
                    SaasUserId = user.SaasUserId,
                    UserName = user.UserName,
                    ConnectionId = Context.ConnectionId,
                    UserAgent = null,
                };

                await _memberService.UpdateActivityAsync(addClientRequest);
                await _clientService.UpdateActivityAsync(addClientRequest);

                return message;
            }),
            request.RequestId);
        }

        public async Task DeleteMessageAsync(DeleteMessageRequest request)
        {
            await CheckAccessTokenAndExecute(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _messageSocketService.DeleteMessageAsync(request);
            }),
            request.RequestId);
        }

        public async Task UpdateMessageAsync(UpdateMessageRequest request)
        {
            await CheckAccessTokenAndExecute(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _messageSocketService.UpdateMessageAsync(request);
            }),
            request.RequestId);
        }

        public async Task AddMessageAttachmentAsync(AddMessageAttachmentRequest request)
        {
            await CheckAccessTokenAndExecute(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _messageSocketService.AddMessageAttachmentAsync(request);
            }),
            request.RequestId);
        }

        public async Task DeleteMessageAttachmentAsync(DeleteMessageAttachmentRequest request)
        {
            await CheckAccessTokenAndExecute(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _messageSocketService.DeleteMessageAttachmentAsync(request);
            }),
            request.RequestId);
        }

        public async Task MarkAsReadMessageAsync(AddLastReadMessageRequest request)
        {
            await CheckAccessTokenAndExecute(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _messageSocketService.AddLastReadMessageAsync(request);
            }), request.RequestId);
        }

        #endregion

        #region Channel Hub Commands

        public async Task JoinToChannelAsync(JoinToChannelRequest request)
        {
            await CheckAccessTokenAndExecute(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _channelSocketService.JoinToChannelAsync(request);
            }),
            request.RequestId);
        }

        public async Task InviteMemberAsync(InviteMemberRequest request)
        {
            await CheckAccessTokenAndExecute(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _channelSocketService.InviteMemberAsync(request);
            }),
            request.RequestId);
        }

        public async Task InviteMembersAsync(InviteMembersRequest request)
        {
            await CheckAccessTokenAndExecute(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _channelSocketService.InviteMembersAsync(request);
            }),
            request.RequestId);
        }

        public async Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest request)
        {
            return await CheckAccessTokenAndExecute(new TaskReference<ChannelSummaryResponse>(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                request.ClientConnectionId = Context.ConnectionId;
                return await _channelSocketService.CreateChannelAsync(request);
            }),
            request.RequestId);
        }

        public async Task<ChannelSummaryResponse> UpdateChannelAsync(UpdateChannelRequest request)
        {
            return await CheckAccessTokenAndExecute(new TaskReference<ChannelSummaryResponse>(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                return await _channelSocketService.UpdateChannelAsync(request);
            }),
            request.RequestId);
        }

        public async Task CloseChannelAsync(ChannelRequest request)
        {
            await CheckAccessTokenAndExecute(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _channelSocketService.CloseChannelAsync(request);
            }),
            request.RequestId);
        }

        public async Task LeaveChannelAsync(ChannelRequest request)
        {
            await CheckAccessTokenAndExecute(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _channelSocketService.LeaveChannelAsync(request);
            }),
            request.RequestId);
        }

        public async Task MuteChannelAsync(ChannelRequest request)
        {
            await CheckAccessTokenAndExecute(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _channelSocketService.MuteChannelAsync(request);
            }),
            request.RequestId);
        }

        #endregion

        #region HubEvents

        async Task IChannelNotificationHub.OnAddChannel(MemberSummary member, ChannelSummaryResponse channel, string clientConnectionId)
        {
            var getClientsExceptCallerRequest = new ChannelRequest(member.SaasUserId, channel.Id)
            {
                ClientConnectionId = clientConnectionId
            };

            var clientIds = await GetChannelClientsExceptCallerAsync(getClientsExceptCallerRequest, clientConnectionId);
            // Tell the people in this room that you've joined
            await Clients.Clients(clientIds).SendAsync(HubEvents.ChannelCreated, channel);
        }

        async Task IChannelNotificationHub.OnUpdateChannel(MemberSummary member, ChannelSummaryResponse channel)
        {
            var clientIds = await GetChannelClientsAsync(new ChannelRequest(member.SaasUserId, channel.Id));

            // Tell the people in this room that you've joined
            await Clients.Clients(clientIds).SendAsync(HubEvents.ChannelUpdated, channel);
        }

        async Task IChannelNotificationHub.OnCloseChannel(MemberSummary member, ChannelSummaryResponse channel)
        {
            var clientIds = await GetChannelClientsAsync(new ChannelRequest(member.SaasUserId, channel.Id));

            // Tell the people in this room that you've joined
            await Clients.Clients(clientIds).SendAsync(HubEvents.ChannelClosed, channel);
        }

        async Task IChannelNotificationHub.OnJoinChannel(MemberSummary member, ChannelSummaryResponse channel)
        {
            var clientIds = await GetChannelClientsAsync(new ChannelRequest(member.SaasUserId, channel.Id));

            // Tell the people in this room that you've joined
            await Clients.Clients(clientIds).SendAsync(HubEvents.MemberJoined, member, channel);
        }

        async Task IChannelNotificationHub.OnLeaveChannel(MemberSummary member, ChannelSummaryResponse channel)
        {
            var clientIds = await GetChannelClientsAsync(new ChannelRequest(member.SaasUserId, channel.Id));
            var senderClients = await _memberService.GetMemberClientsAsync(member.Id);
            clientIds.AddRange(senderClients.Select(x => x.ClientConnectionId));

            // Tell the people in this room that you've leaved
            await Clients.Clients(clientIds).SendAsync(HubEvents.MemberLeft, member, channel?.Id);
        }

        async Task IMessageNotificationHub.OnAddMessage(MemberSummary member, MessageResponse message, string clientConnectionId)
        {
            var getChannelClientsExceptCallerRequest = new ChannelRequest(member.SaasUserId, message.ChannelId);

            var clientIds = await GetChannelClientsExceptCallerAsync(getChannelClientsExceptCallerRequest, clientConnectionId);

            // Notify all clients for the uploaded message
            await Clients.Clients(clientIds).SendAsync(HubEvents.MessageAdded, message);
        }

        async Task IMessageNotificationHub.OnDeleteMessage(MemberSummary member, MessageResponse message)
        {
            var channelSummary = await _channelService.GetChannelSummaryAsync(new ChannelRequest(member.SaasUserId, message.ChannelId));

            var clientIds = await GetChannelClientsAsync(new ChannelRequest(member.SaasUserId, channelSummary.Id));

            // Notify all clients for the deleted message
            await Clients.Clients(clientIds).SendAsync(HubEvents.MessageDeleted, message.Id, channelSummary);
        }

        async Task IMessageNotificationHub.OnUpdateMessage(MemberSummary member, MessageResponse message)
        {
            var channel = await _channelService.GetChannelByIdAsync(new ChannelRequest(member.SaasUserId, message.ChannelId));

            var clientIds = await GetChannelClientsAsync(new ChannelRequest(member.SaasUserId, channel.Id));

            // Notify all clients for the deleted message
            await Clients.Clients(clientIds).SendAsync(HubEvents.MessageUpdated, message);
        }

        async Task IMessageNotificationHub.OnAddMessageAttachment(MemberSummary member, MessageResponse message)
        {
            var channel = await _channelService.GetChannelByIdAsync(new ChannelRequest(member.SaasUserId, message.ChannelId));

            var clientIds = await GetChannelClientsAsync(new ChannelRequest(member.SaasUserId, channel.Id));

            // Notify all clients for the uploaded message
            await Clients.Clients(clientIds).SendAsync(HubEvents.AttachmentAdded, channel.Name);
        }

        async Task IMessageNotificationHub.OnDeleteMessageAttachment(MemberSummary member, MessageResponse message)
        {
            var channel = await _channelService.GetChannelByIdAsync(new ChannelRequest(member.SaasUserId, message.ChannelId));

            var clientIds = await GetChannelClientsAsync(new ChannelRequest(member.SaasUserId, channel.Id));

            // Notify all clients for the uploaded message
            await Clients.Clients(clientIds).SendAsync(HubEvents.AttachmentDeleted, channel.Name);
        }

        async Task IMessageNotificationHub.OnChangeLastReadMessage(List<MemberSummary> members, MessageResponse message)
        {
            var channelRequest = new ChannelRequest(members.First().SaasUserId, message.ChannelId);

            var channel = await _channelService.GetChannelByIdAsync(channelRequest);

            var connectionIds = await GetNotMutedChannelMembersConnectionsAsync(channelRequest, members.Select(x => x.Id));

            // Notify owner about read message
            await Clients.Clients(connectionIds).SendAsync(HubEvents.LastReadMessageChanged, channel.Name);
        }

        private async Task<List<string>> GetChannelClientsAsync(ChannelRequest request)
        {
            // TODO: Change this code. Recommended to use Clients.Group()
            var members = await _channelMemberService.GetChannelMembersAsync(request);

            return await FilterClients(members, request.ClientConnectionId);
        }

        private async Task<List<string>> GetChannelClientsExceptCallerAsync(ChannelRequest request, string callerConnectionId)
        {
            // TODO: Change this code. Recommended to use Clients.Group()
            var members = await _channelMemberService.GetChannelMembersAsync(request);

            var mutedMemberIds = members.Where(x => x.IsMuted)
                .Select(x => x.MemberId)
                .ToList();

            var mutedConnectionClientIds = (await _memberService.GetClientsByMemberIds(mutedMemberIds))
                .Select(x => x.ConnectionClientId)
                .ToList();
            mutedConnectionClientIds.Add(callerConnectionId);

            var clients = new List<string>();
            foreach (var item in members)
            {
                var memberClients = (await _memberService.GetMemberClientsAsync(item.MemberId))
                    .Select(x => x.ClientConnectionId)
                    .Except(mutedConnectionClientIds)
                    .ToList();

                clients.AddRange(memberClients);
            }

            // TODO: clear connectionIds in database. There are about 4000 connections for only 3 users at the moment
            return clients;
        }

        private async Task<List<string>> FilterClients(IEnumerable<ChannelMemberResponse> members, string clientConnectionId)
        {
            var mutedMemberIds = members.Where(x => x.IsMuted)
                .Select(x => x.MemberId)
                .ToList();

            var mutedConnectionClientIds = (await _memberService.GetClientsByMemberIds(mutedMemberIds))
                .Select(x => x.ConnectionClientId)
                .ToList();

            var clients = new List<string>();
            foreach (var item in members)
            {
                var memberClients = (await _memberService.GetMemberClientsAsync(item.MemberId))
                    .Where(x => x.ClientConnectionId != clientConnectionId)
                    .Select(x => x.ClientConnectionId)
                    .Except(mutedConnectionClientIds)
                    .ToList();

                clients.AddRange(memberClients);
            }

            return clients;
        }

        private async Task<List<string>> GetNotMutedChannelMembersConnectionsAsync(ChannelRequest request, IEnumerable<Guid> notifyMemberIds)
        {
            var channelMembers = await _channelMemberService.GetChannelMembersAsync(new ChannelRequest(request.SaasUserId, request.ChannelId));

            var notMutedMemberIds = channelMembers.Where(x => !x.IsMuted && notifyMemberIds.Contains(x.MemberId))
                .Select(x => x.MemberId)
                .ToList();

            var notMutedConnectionClientIds = (await _memberService.GetClientsByMemberIds(notMutedMemberIds))
                .Select(x => x.ConnectionClientId)
                .ToList();

            return notMutedConnectionClientIds;
        }

        private async Task CheckAccessTokenAndExecute(TaskReference funcRequest, string requestId = null)
        {
            var saasUserId = Context.GetSaasUserId();
            if (saasUserId != null)
            {
                try
                {
                    await funcRequest.RunAsync();
                    await Clients.Caller.SendAsync(HubEvents.RequestSuccess, requestId);
                }
                catch (Exception ex)
                {
                    if (requestId == null)
                    {
                        throw;
                    }
                    await Clients.Caller.SendAsync(HubEvents.ExceptionOccurred, ex, requestId);
                }
            }
            else
            {
                await Clients.Caller.SendAsync(HubEvents.AccessTokenExpired, requestId);
            }
        }

        private async Task<T> CheckAccessTokenAndExecute<T>(TaskReference<T> funcRequest, string requestId = null)
        {
            var saasUserId = Context.GetSaasUserId();
            if (saasUserId != null)
            {
                try
                {
                    await Clients.Caller.SendAsync(HubEvents.RequestSuccess, requestId);
                    return await funcRequest.RunAsync();
                }
                catch (Exception ex)
                {
                    if (requestId == null)
                    {
                        throw;
                    }
                    await Clients.Caller.SendAsync(HubEvents.ExceptionOccurred, ex, requestId);
                    return default(T);
                }
            }

            await Clients.Caller.SendAsync(HubEvents.AccessTokenExpired, requestId);
            return default(T);
        }

        #endregion
    }
}