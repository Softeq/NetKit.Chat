// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Softeq.NetKit.Chat.Domain.Services;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.MessageAttachment;
using Softeq.NetKit.Chat.Domain.TransportModels.Response;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Softeq.NetKit.Chat.SignalR.Sockets;
using Softeq.Serilog.Extension;

namespace Softeq.NetKit.Chat.SignalR.Hubs
{
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
            return await CheckAccessTokenAndExecute(new TaskReference<ClientResponse>(async () =>
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
            await CheckAccessTokenAndExecute(new TaskReference(async () =>
            {
                var deleteClientRequest = new DeleteClientRequest(Context.ConnectionId);
                await _memberService.DeleteClientAsync(deleteClientRequest);
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
                return await _messageSocketService.AddMessageAsync(request);
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

        public async Task MarkAsReadMessageAsync(SetLastReadMessageRequest request)
        {
            await CheckAccessTokenAndExecute(new TaskReference(async () =>
            {
                request.SaasUserId = Context.GetSaasUserId();
                await _messageSocketService.SetLastReadMessageAsync(request);
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
    }
}