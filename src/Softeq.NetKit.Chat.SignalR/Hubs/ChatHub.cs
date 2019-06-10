// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnsureThat;
using FluentValidation;
using FluentValidation.Results;
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
using Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Channel;
using Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Member;
using Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Message;
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
            _logger.Event("SignalRClientConnected").With.Message("{@ConnectionId}", Context.ConnectionId).AsInformation();

            // TODO [az]: should we create client here?

            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.Event("SignalRClientDisconnected").With.Message("{@ConnectionId}", Context.ConnectionId).AsInformation();

            await _clientService.DeleteClientAsync(new DeleteClientRequest(Context.ConnectionId));

            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        #region Client Hub Commands

        public async Task<ClientResponse> GetClientAsync()
        {
            return await SafeExecuteAsync(new TaskReference<ClientResponse>(async () =>
            {
                var getClientRequest = new GetClientRequest(Context.ConnectionId);
                return await _clientService.GetClientAsync(getClientRequest);
            }));
        }

        public async Task<ClientResponse> AddClientAsync()
        {
            return await SafeExecuteAsync(new TaskReference<ClientResponse>(async () =>
            {
                var addClientRequest = new AddClientRequest(Context.GetSaasUserId(), Context.GetUserName(), Context.ConnectionId, null, Context.GetEmail());
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
            return await ValidateAndExecuteAsync(request, new AddMessageRequestValidator(), new TaskReference<MessageResponse>(async () =>
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
            await ValidateAndExecuteAsync(request, new DeleteMessageRequestValidator(), new TaskReference(async () =>
            {
                var archiveMessageRequest = new ArchiveMessageRequest(Context.GetSaasUserId(), request.MessageId);
                await _messageSocketService.ArchiveMessageAsync(archiveMessageRequest);
            }),
            request.RequestId);
        }

        public async Task UpdateMessageAsync(SignalRRequest.Message.UpdateMessageRequest request)
        {
            await ValidateAndExecuteAsync(request, new UpdateMessageRequestValidator(), new TaskReference(async () =>
            {
                var updateMessageRequest = new DomainRequest.Message.UpdateMessageRequest(Context.GetSaasUserId(), request.MessageId, request.Body);
                await _messageSocketService.UpdateMessageAsync(updateMessageRequest);
            }),
            request.RequestId);
        }

        public async Task MarkAsReadMessageAsync(SignalRRequest.Message.SetLastReadMessageRequest request)
        {
            await ValidateAndExecuteAsync(request, new SetLastReadMessageRequestValidator(), new TaskReference(async () =>
            {
                var setLastReadMessageRequest = new DomainRequest.Message.SetLastReadMessageRequest(Context.GetSaasUserId(), request.ChannelId, request.MessageId);
                await _messageSocketService.SetLastReadMessageAsync(setLastReadMessageRequest);
            }), request.RequestId);
        }

        #endregion

        #region Member Hub Commands

        public async Task InviteMemberAsync(SignalRRequest.Member.InviteMemberRequest request)
        {
            await ValidateAndExecuteAsync(request, new InviteMemberRequestValidator(), new TaskReference(async () =>
            {
                var inviteMemberRequest = new DomainRequest.Member.InviteMemberRequest(Context.GetSaasUserId(), request.ChannelId, request.MemberId);
                await _channelSocketService.InviteMemberAsync(inviteMemberRequest);
            }),
            request.RequestId);
        }

        public async Task InviteMultipleMembersAsync(SignalRRequest.Member.InviteMultipleMembersRequest request)
        {
            await ValidateAndExecuteAsync(request, new InviteMultipleMembersRequestValidator(), new TaskReference(async () =>
            {
                var inviteMultipleMembersRequest = new DomainRequest.Member.InviteMultipleMembersRequest(Context.GetSaasUserId(), request.ChannelId, request.InvitedMembersIds);
                await _channelSocketService.InviteMultipleMembersAsync(inviteMultipleMembersRequest);
            }),
            request.RequestId);
        }

        public async Task DeleteMemberAsync(SignalRRequest.Member.DeleteMemberRequest request)
        {
            await ValidateAndExecuteAsync(request, new DeleteMemberRequestValidator(), new TaskReference(async () =>
            {
                var deleteMemberRequest = new DomainRequest.Member.DeleteMemberRequest(Context.GetSaasUserId(), request.ChannelId, request.MemberId);
                await _channelSocketService.DeleteMemberFromChannelAsync(deleteMemberRequest);
            }),
            request.RequestId);
        }

        #endregion

        #region Channel Hub Commands

        public async Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest request)
        {
            return await ValidateAndExecuteAsync(request, new CreateChannelRequestValidator(), new TaskReference<ChannelSummaryResponse>(async () =>
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

        public async Task<ChannelSummaryResponse> CreateDirectChannelAsync(CreateDirectChannelRequest request)
        {
            return await ValidateAndExecuteAsync(request, new CreateDirectChannelRequestValidator(), new TaskReference<ChannelSummaryResponse>(async () =>
                {
                    var createChannelRequest =
                        new DomainRequest.Channel.CreateDirectChannelRequest(Context.GetSaasUserId(), request.MemberId);

                    return await _channelSocketService.CreateDirectChannelAsync(createChannelRequest);
                }),
                request.RequestId);
        }

        public async Task<ChannelSummaryResponse> UpdateChannelAsync(UpdateChannelRequest request)
        {
            return await ValidateAndExecuteAsync(request, new UpdateChannelRequestValidator(), new TaskReference<ChannelSummaryResponse>(async () =>
            {
                var updateChannelRequest = new DomainRequest.Channel.UpdateChannelRequest(Context.GetSaasUserId(), request.ChannelId, request.Name)
                {
                    PhotoUrl = request.PhotoUrl,
                    Description = request.Description,
                    WelcomeMessage = request.WelcomeMessage
                };
                return await _channelSocketService.UpdateChannelAsync(updateChannelRequest);
            }),
            request.RequestId);
        }

        public async Task CloseChannelAsync(ChannelRequest request)
        {
            await ValidateAndExecuteAsync(request, new ChannelRequestValidator(), new TaskReference(async () =>
            {
                var closeChannelRequest = new DomainRequest.Channel.ChannelRequest(Context.GetSaasUserId(), request.ChannelId);
                await _channelSocketService.CloseChannelAsync(closeChannelRequest);
            }),
            request.RequestId);
        }

        public async Task LeaveChannelAsync(ChannelRequest request)
        {
            await ValidateAndExecuteAsync(request, new ChannelRequestValidator(), new TaskReference(async () =>
            {
                var closeChannelRequest = new DomainRequest.Channel.ChannelRequest(Context.GetSaasUserId(), request.ChannelId);
                await _channelSocketService.LeaveChannelAsync(closeChannelRequest);
            }),
            request.RequestId);
        }

        public async Task MuteChannelAsync(MuteChannelRequest request)
        {
            await ValidateAndExecuteAsync(request, new MuteChannelRequestValidator(), new TaskReference(async () =>
            {
                await _channelService.MuteChannelAsync(Context.GetSaasUserId(), request.ChannelId, request.IsMuted);
            }),
            request.RequestId);
        }

        public async Task PinChannelAsync(PinChannelRequest request)
        {
            await ValidateAndExecuteAsync(request, new PinChannelRequestValidator(), new TaskReference(async () =>
            {
                await _channelService.PinChannelAsync(Context.GetSaasUserId(), request.ChannelId, request.IsPinned);
            }),
            request.RequestId);
        }

        #endregion

        private async Task ValidateAndExecuteAsync<TRequest>(TRequest request, IValidator<TRequest> requestValidator, TaskReference funcRequest, string requestId = null)
        {
            if (requestValidator != null)
            {
                if (request == null)
                {
                    var failures = new List<ValidationFailure> { new ValidationFailure(nameof(request), "can not be null") };
                    await Clients.Caller.SendAsync(HubEvents.RequestValidationFailed, failures, requestId);
                    return;
                }

                var validationResult = await requestValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    await Clients.Caller.SendAsync(HubEvents.RequestValidationFailed, validationResult.Errors, requestId);
                    return;
                }
            }

            await SafeExecuteAsync(funcRequest, requestId);
        }

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

        private async Task<TResponse> ValidateAndExecuteAsync<TRequest, TResponse>(TRequest request, IValidator<TRequest> requestValidator, TaskReference<TResponse> funcRequest, string requestId = null)
        {
            if (requestValidator != null)
            {
                if (request == null)
                {
                    var failures = new List<ValidationFailure> { new ValidationFailure(nameof(request), "can not be null") };
                    await Clients.Caller.SendAsync(HubEvents.RequestValidationFailed, failures, requestId);
                    return default(TResponse);
                }

                var validationResult = await requestValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    await Clients.Caller.SendAsync(HubEvents.RequestValidationFailed, validationResult.Errors, requestId);
                    return default(TResponse);
                }
            }

            return await SafeExecuteAsync(funcRequest, requestId);
        }

        private async Task<TResponse> SafeExecuteAsync<TResponse>(TaskReference<TResponse> funcRequest, string requestId = null)
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
                    return default(TResponse);
                }
            }

            await Clients.Caller.SendAsync(HubEvents.AccessTokenExpired, requestId);
            return default(TResponse);
        }
    }
}
