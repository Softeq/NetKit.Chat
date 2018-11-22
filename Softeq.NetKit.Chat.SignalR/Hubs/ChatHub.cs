﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Member;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Softeq.NetKit.Chat.SignalR.Sockets;
using Softeq.Serilog.Extension;

namespace Softeq.NetKit.Chat.SignalR.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMemberService _memberService;
        private readonly ILogger _logger;
        private readonly IChannelSocketService _channelSocketService;
        private readonly IMessageSocketService _messageSocketService;

        public ChatHub(IMemberService memberService,
                       ILogger logger,
                       IChannelSocketService channelSocketService,
                       IMessageSocketService messageSocketService)
        {
            _memberService = memberService;
            _logger = logger;
            _channelSocketService = channelSocketService;
            _messageSocketService = messageSocketService;
        }

        #region Override

        public override Task OnConnectedAsync()
        {
            _logger.Event("SignalRClientConnected").With.Message("{@ConnectionId}", Context.ConnectionId).AsInformation();

            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var deleteClientRequest = new DeleteClientRequest(Context.ConnectionId);
            await _memberService.DeleteClientAsync(deleteClientRequest);

            _logger.Event("SignalRClientDisconnected").With.Message("{@ConnectionId}", Context.ConnectionId).AsInformation();
            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        #region Client Hub Commands

        public async Task<ClientResponse> AddClientAsync()
        {
            return await SafeExecuteAsync(new TaskReference<ClientResponse>(async () =>
            {
                var addClientRequest = new AddClientRequest
                {
                    ConnectionId = Context.ConnectionId,
                    UserAgent = null,
                    UserName = Context.GetUserName(),
                    SaasUserId = Context.GetSaasUserId()
                };

                return await _memberService.GetOrAddClientAsync(addClientRequest);
            }));
        }

        public async Task DeleteClientAsync()
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                var deleteClientRequest = new DeleteClientRequest(Context.ConnectionId);
                await _memberService.DeleteClientAsync(deleteClientRequest);
            }));
        }

        #endregion

        #region Message Hub Commands

        public async Task<MessageResponse> AddMessageAsync(CreateMessageRequest request)
        {
            return await SafeExecuteAsync(new TaskReference<MessageResponse>(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                request.ClientConnectionId = Context.ConnectionId;
                return await _messageSocketService.AddMessageAsync(request);
            }),
            request.RequestId);
        }

        public async Task DeleteMessageAsync(DeleteMessageRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _messageSocketService.DeleteMessageAsync(request);
            }),
            request.RequestId);
        }

        public async Task UpdateMessageAsync(UpdateMessageRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _messageSocketService.UpdateMessageAsync(request);
            }),
            request.RequestId);
        }

        public async Task AddMessageAttachmentAsync(AddMessageAttachmentRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _messageSocketService.AddMessageAttachmentAsync(request);
            }),
            request.RequestId);
        }

        public async Task DeleteMessageAttachmentAsync(DeleteMessageAttachmentRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _messageSocketService.DeleteMessageAttachmentAsync(request);
            }),
            request.RequestId);
        }

        public async Task MarkAsReadMessageAsync(SetLastReadMessageRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _messageSocketService.SetLastReadMessageAsync(request);
            }), request.RequestId);
        }

        #endregion

        #region Channel Hub Commands

        public async Task JoinToChannelAsync(JoinToChannelRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _channelSocketService.JoinToChannelAsync(request);
            }),
            request.RequestId);
        }

        public async Task InviteMemberAsync(InviteMemberRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _channelSocketService.InviteMemberAsync(request);
            }),
            request.RequestId);
        }

        public async Task InviteMultipleMembersAsync(InviteMembersRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _channelSocketService.InviteMultipleMembersAsync(request);
            }),
            request.RequestId);
        }

        public async Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest request)
        {
            return await SafeExecuteAsync(new TaskReference<ChannelSummaryResponse>(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                request.ClientConnectionId = Context.ConnectionId;
                return await _channelSocketService.CreateChannelAsync(request);
            }),
            request.RequestId);
        }

        public async Task<ChannelSummaryResponse> UpdateChannelAsync(UpdateChannelRequest request)
        {
            return await SafeExecuteAsync(new TaskReference<ChannelSummaryResponse>(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                return await _channelSocketService.UpdateChannelAsync(request);
            }),
            request.RequestId);
        }

        public async Task CloseChannelAsync(ChannelRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _channelSocketService.CloseChannelAsync(request);
            }),
            request.RequestId);
        }

        public async Task LeaveChannelAsync(ChannelRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _channelSocketService.LeaveChannelAsync(request);
            }),
            request.RequestId);
        }

        public async Task DeleteMemberAsync(DeleteMemberRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
                {
                    request.SaasUserId = Context.GetSaasUserId();
                    await _channelSocketService.DeleteMemberAsync(request);
                }),
                request.RequestId);
        }

        public async Task MuteChannelAsync(ChannelRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _channelSocketService.MuteChannelAsync(request);
            }),
            request.RequestId);
        }

        public async Task PinChannelAsync(ChannelRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _channelSocketService.PinChannelAsync(request);
            }),
            request.RequestId);
        }

        #endregion

        private async Task SafeExecuteAsync(TaskReference funcRequest, string requestId = null)
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

        private async Task<T> SafeExecuteAsync<T>(TaskReference<T> funcRequest, string requestId = null)
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
    }
}