// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Message;
using Softeq.NetKit.Chat.SignalR.Sockets;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Message;
using Softeq.Serilog.Extension;
using DomainRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request;
using SignalRRequest = Softeq.NetKit.Chat.SignalR.TransportModels.Request;

namespace Softeq.NetKit.Chat.SignalR.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger _logger;
        private readonly IChannelSocketService _channelSocketService;
        private readonly IMessageSocketService _messageSocketService;
        private readonly IChannelService _channelService;
        private readonly IClientService _clientService;

        public ChatHub(ILogger logger,
                       IChannelSocketService channelSocketService,
                       IMessageSocketService messageSocketService,
                       IChannelService channelService,
                       IClientService clientService)
        {
            Ensure.That(logger).IsNotNull();
            Ensure.That(channelSocketService).IsNotNull();
            Ensure.That(messageSocketService).IsNotNull();
            Ensure.That(channelService).IsNotNull();
            Ensure.That(clientService).IsNotNull();

            _logger = logger;
            _channelSocketService = channelSocketService;
            _messageSocketService = messageSocketService;
            _channelService = channelService;
            _clientService = clientService;
        }

        #region Override

        public override Task OnConnectedAsync()
        {
            // TODO [az]: should we create client here?

            _logger.Event("SignalRClientConnected").With.Message("{@ConnectionId}", Context.ConnectionId).AsInformation();
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _clientService.DeleteClientAsync(new DeleteClientRequest(Context.ConnectionId));

            _logger.Event("SignalRClientDisconnected").With.Message("{@ConnectionId}", Context.ConnectionId).AsInformation();
            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        #region Client Hub Commands

        public async Task<ClientResponse> GetClientAsync()
        {
            var getClientRequest = new GetClientRequest(Context.GetSaasUserId(), Context.ConnectionId);
            return await SafeExecuteAsync(new TaskReference<ClientResponse>(async () => await _clientService.GetClientAsync(getClientRequest)));
        }

        public async Task<ClientResponse> AddClientAsync()
        {
            return await SafeExecuteAsync(new TaskReference<ClientResponse>(async () =>
            {
                var addClientRequest = new AddClientRequest(Context.GetSaasUserId(), Context.GetUserName(), Context.ConnectionId, null);
                return await _clientService.AddClientAsync(addClientRequest);
            }));
        }

        public async Task DeleteClientAsync()
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                var deleteClientRequest = new DeleteClientRequest(Context.ConnectionId);
                await _clientService.DeleteClientAsync(deleteClientRequest);
            }));
        }

        #endregion

        #region Message Hub Commands

        public async Task<MessageResponse> AddMessageAsync(AddMessageRequest request)
        {
            return await SafeExecuteAsync(new TaskReference<MessageResponse>(async () =>
            {
                var createMessageRequest = new CreateMessageRequest(Context.GetSaasUserId(), request.ChannelId, request.Type, request.Body)
                {
                    ImageUrl = request.ImageUrl,
                    ForwardedMessageId = request.ForwardedMessageId
                };
                return await _messageSocketService.AddMessageAsync(createMessageRequest, Context.ConnectionId);
            }),
            request.RequestId);
        }

        public async Task DeleteMessageAsync(SignalRRequest.Message.DeleteMessageRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                var deleteMessageRequest = new DomainRequest.Message.DeleteMessageRequest(Context.GetSaasUserId(), request.MessageId);
                await _messageSocketService.DeleteMessageAsync(deleteMessageRequest);
            }),
            request.RequestId);
        }

        public async Task UpdateMessageAsync(SignalRRequest.Message.UpdateMessageRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                var updateMessageRequest = new DomainRequest.Message.UpdateMessageRequest(Context.GetSaasUserId(), request.MessageId, request.Body);
                await _messageSocketService.UpdateMessageAsync(updateMessageRequest);
            }),
            request.RequestId);
        }

        public async Task MarkAsReadMessageAsync(SignalRRequest.Message.SetLastReadMessageRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                var setLastReadMessageRequest = new DomainRequest.Message.SetLastReadMessageRequest(Context.GetSaasUserId(), request.ChannelId, request.MessageId);
                await _messageSocketService.SetLastReadMessageAsync(setLastReadMessageRequest);
            }), request.RequestId);
        }

        #endregion

        #region Message Attachment Hub Commands

        public async Task AddMessageAttachmentAsync(SignalRRequest.MessageAttachment.AddMessageAttachmentRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                var addMessageAttachmentRequest = new DomainRequest.MessageAttachment.AddMessageAttachmentRequest(Context.GetSaasUserId(), request.MessageId, request.Content, request.Extension, request.ContentType, request.Size);
                await _messageSocketService.AddMessageAttachmentAsync(addMessageAttachmentRequest);
            }),
            request.RequestId);
        }

        public async Task DeleteMessageAttachmentAsync(SignalRRequest.MessageAttachment.DeleteMessageAttachmentRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                var deleteMessageAttachmentRequest = new DomainRequest.MessageAttachment.DeleteMessageAttachmentRequest(Context.GetSaasUserId(), request.MessageId, request.AttachmentId);
                await _messageSocketService.DeleteMessageAttachmentAsync(deleteMessageAttachmentRequest);
            }),
            request.RequestId);
        }

        #endregion

        #region Channel Hub Commands

        public async Task JoinToChannelAsync(SignalRRequest.Channel.ChannelRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                var joinToChannelRequest = new DomainRequest.Channel.ChannelRequest(Context.GetSaasUserId(), request.ChannelId);
                await _channelSocketService.JoinToChannelAsync(joinToChannelRequest);
            }),
            request.RequestId);
        }

        public async Task InviteMemberAsync(SignalRRequest.Member.InviteMemberRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                var inviteMemberRequest = new DomainRequest.Member.InviteMemberRequest(Context.GetSaasUserId(), request.ChannelId, request.MemberId);
                await _channelSocketService.InviteMemberAsync(inviteMemberRequest);
            }),
            request.RequestId);
        }

        public async Task InviteMultipleMembersAsync(SignalRRequest.Member.InviteMultipleMembersRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                var inviteMultipleMembersRequest = new DomainRequest.Member.InviteMultipleMembersRequest(Context.GetSaasUserId(), request.ChannelId, request.InvitedMembersIds);
                await _channelSocketService.InviteMultipleMembersAsync(inviteMultipleMembersRequest);
            }),
            request.RequestId);
        }

        public async Task DeleteMemberAsync(SignalRRequest.Member.DeleteMemberRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                var deleteMemberRequest = new DomainRequest.Member.DeleteMemberRequest(Context.GetSaasUserId(), request.ChannelId, request.MemberId);
                await _channelSocketService.DeleteMemberFromChannelAsync(deleteMemberRequest);
            }),
            request.RequestId);
        }

        public async Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest request)
        {
            return await SafeExecuteAsync(new TaskReference<ChannelSummaryResponse>(async () =>
            {
                var createChannelRequest = new DomainRequest.Channel.CreateChannelRequest(Context.GetSaasUserId(), request.Name, request.Type)
                {
                    AllowedMembers = request.AllowedMembers,
                    Description = request.Description,
                    PhotoUrl = request.PhotoUrl,
                    WelcomeMessage = request.WelcomeMessage
                };
                return await _channelSocketService.CreateChannelAsync(createChannelRequest);
            }),
            request.RequestId);
        }

        public async Task<ChannelSummaryResponse> UpdateChannelAsync(UpdateChannelRequest request)
        {
            return await SafeExecuteAsync(new TaskReference<ChannelSummaryResponse>(async () =>
            {
                var updateChannelRequest = new DomainRequest.Channel.UpdateChannelRequest(Context.GetSaasUserId(), request.ChannelId, request.Name)
                {
                    PhotoUrl = request.PhotoUrl,
                    Topic = request.Topic,
                    WelcomeMessage = request.WelcomeMessage
                };
                return await _channelSocketService.UpdateChannelAsync(updateChannelRequest);
            }),
            request.RequestId);
        }

        public async Task CloseChannelAsync(ChannelRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                var closeChannelRequest = new DomainRequest.Channel.ChannelRequest(Context.GetSaasUserId(), request.ChannelId);
                await _channelSocketService.CloseChannelAsync(closeChannelRequest);
            }),
            request.RequestId);
        }

        public async Task LeaveChannelAsync(ChannelRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                var closeChannelRequest = new DomainRequest.Channel.ChannelRequest(Context.GetSaasUserId(), request.ChannelId);
                await _channelSocketService.LeaveChannelAsync(closeChannelRequest);
            }),
            request.RequestId);
        }

        public async Task MuteChannelAsync(MuteChannelRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                await _channelService.MuteChannelAsync(Context.GetSaasUserId(), request.ChannelId, request.IsMuted);
            }),
            request.RequestId);
        }

        public async Task PinChannelAsync(PinChannelRequest request)
        {
            await SafeExecuteAsync(new TaskReference(async () =>
            {
                await _channelService.PinChannelAsync(Context.GetSaasUserId(), request.ChannelId, request.IsPinned);
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
                    var result = await funcRequest.RunAsync();
                    await Clients.Caller.SendAsync(HubEvents.RequestSuccess, requestId);
                    return result;
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