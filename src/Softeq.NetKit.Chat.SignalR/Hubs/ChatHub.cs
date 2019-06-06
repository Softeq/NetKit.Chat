// Developed by Softeq Development Corporation
// http://www.softeq.com

using EnsureThat;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Softeq.NetKit.Chat.Client.SDK.Models.SignalRModels;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Request.Message;
using Softeq.NetKit.Chat.SignalR.Sockets;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Message;
using Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Channel;
using Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Member;
using Softeq.NetKit.Chat.SignalR.TransportModels.Validators.Message;
using Softeq.Serilog.Extension;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Request.Client;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Message;
using Softeq.NetKit.Chat.Client.SDK.Models.SignalRModels.Client;
using AddClientRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request.Client.AddClientRequest;
using Channel = Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Request.Channel;
using DeleteMessageRequest = Softeq.NetKit.Chat.SignalR.TransportModels.Request.Message.DeleteMessageRequest;
using DomainRequest = Softeq.NetKit.Chat.Domain.TransportModels.Request;
using Member = Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Request.Member;
using Message = Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Request.Message;
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

        public async Task<MessageResponse> AddMessageAsync(SignalRRequest<Message.AddMessageRequest> request)
        {
            var addMessageResponse = new AddMessageRequest
            {
                Body = request.Request.Body,
                ChannelId = request.Request.ChannelId,
                ForwardedMessageId = request.Request.ForwardedMessageId,
                ImageUrl = request.Request.ImageUrl,
                RequestId = request.RequestId,
                Type = MessageTypeConvertor.Convert(request.Request.Type)
            };

            return await ValidateAndExecuteAsync(addMessageResponse, new AddMessageRequestValidator(), new TaskReference<MessageResponse>(async () =>
            {
                var createMessageRequest = new CreateMessageRequest(Context.GetSaasUserId(), addMessageResponse.ChannelId, addMessageResponse.Type, addMessageResponse.Body)
                {
                    ImageUrl = addMessageResponse.ImageUrl,
                    ForwardedMessageId = addMessageResponse.ForwardedMessageId
                };
                return await _messageSocketService.AddMessageAsync(createMessageRequest, Context.ConnectionId);
            }),
            request.RequestId);
        }

        public async Task DeleteMessageAsync(SignalRRequest<DeleteMessageRequest> request)
        {
            var deleteRequest = new DeleteMessageRequest
            {
                MessageId = request.Request.MessageId,
                RequestId = request.RequestId
            };

            await ValidateAndExecuteAsync(deleteRequest, new DeleteMessageRequestValidator(), new TaskReference(async () =>
            {
                var archiveMessageRequest = new ArchiveMessageRequest(Context.GetSaasUserId(), deleteRequest.MessageId);
                await _messageSocketService.ArchiveMessageAsync(archiveMessageRequest);
            }),
            request.RequestId);
        }

        public async Task UpdateMessageAsync(SignalRRequest<Message.UpdateMessageRequest> request)
        {
            var updateRequest = new SignalRRequest.Message.UpdateMessageRequest
            {
                Body = request.Request.Body,
                MessageId = request.Request.MessageId,
                RequestId = request.RequestId
            };

            await ValidateAndExecuteAsync(updateRequest, new UpdateMessageRequestValidator(), new TaskReference(async () =>
            {
                var updateMessageRequest = new DomainRequest.Message.UpdateMessageRequest(Context.GetSaasUserId(), updateRequest.MessageId, updateRequest.Body);
                await _messageSocketService.UpdateMessageAsync(updateMessageRequest);
            }),
            request.RequestId);
        }

        public async Task MarkAsReadMessageAsync(SignalRRequest<Message.SetLastReadMessageRequest> request)
        {
            var lastReadMessageRequest = new SignalRRequest.Message.SetLastReadMessageRequest
            {
                ChannelId = request.Request.ChannelId,
                MessageId = request.Request.MessageId,
                RequestId = request.RequestId
            };

            await ValidateAndExecuteAsync(lastReadMessageRequest, new SetLastReadMessageRequestValidator(), new TaskReference(async () =>
            {
                var setLastReadMessageRequest = new DomainRequest.Message.SetLastReadMessageRequest(Context.GetSaasUserId(), lastReadMessageRequest.ChannelId, lastReadMessageRequest.MessageId);
                await _messageSocketService.SetLastReadMessageAsync(setLastReadMessageRequest);
            }), request.RequestId);
        }

        #endregion

        #region Member Hub Commands

        public async Task InviteMemberAsync(SignalRRequest<Member.InviteMemberRequest> request)
        {
            var inviteRequest = new SignalRRequest.Member.InviteMemberRequest
            {
                ChannelId = request.Request.ChannelId,
                MemberId = request.Request.MemberId,
                RequestId = request.RequestId
            };

            await ValidateAndExecuteAsync(inviteRequest, new InviteMemberRequestValidator(), new TaskReference(async () =>
            {
                var inviteMemberRequest = new DomainRequest.Member.InviteMemberRequest(Context.GetSaasUserId(), inviteRequest.ChannelId, inviteRequest.MemberId);
                await _channelSocketService.InviteMemberAsync(inviteMemberRequest);
            }),
            request.RequestId);
        }

        public async Task InviteMultipleMembersAsync(SignalRRequest<Member.InviteMultipleMembersRequest> request)
        {
            var multipleMembersRequest = new SignalRRequest.Member.InviteMultipleMembersRequest
            {
                ChannelId = request.Request.ChannelId,
                InvitedMembersIds = request.Request.InvitedMembersIds,
                RequestId = request.RequestId
            };

            await ValidateAndExecuteAsync(multipleMembersRequest, new InviteMultipleMembersRequestValidator(), new TaskReference(async () =>
            {
                var inviteMultipleMembersRequest = new DomainRequest.Member.InviteMultipleMembersRequest(Context.GetSaasUserId(), multipleMembersRequest.ChannelId, multipleMembersRequest.InvitedMembersIds);
                await _channelSocketService.InviteMultipleMembersAsync(inviteMultipleMembersRequest);
            }),
            request.RequestId);
        }

        public async Task DeleteMemberAsync(SignalRRequest<Member.DeleteMemberRequest> request)
        {
            var deleteRequest = new SignalRRequest.Member.DeleteMemberRequest
            {
                ChannelId = request.Request.ChannelId,
                MemberId = request.Request.MemberId,
                RequestId = request.RequestId
            };

            await ValidateAndExecuteAsync(deleteRequest, new DeleteMemberRequestValidator(), new TaskReference(async () =>
            {
                var deleteMemberRequest = new DomainRequest.Member.DeleteMemberRequest(Context.GetSaasUserId(), deleteRequest.ChannelId, deleteRequest.MemberId);
                await _channelSocketService.DeleteMemberFromChannelAsync(deleteMemberRequest);
            }),
            request.RequestId);
        }

        #endregion

        #region Channel Hub Commands

        public async Task<ChannelSummaryResponse> CreateChannelAsync(SignalRRequest<Channel.CreateChannelRequest> request)
        {
            var channelRequest = new CreateChannelRequest()
            {
                AllowedMembers = request.Request.AllowedMembers,
                Description = request.Request.Description,
                Name = request.Request.Name,
                PhotoUrl = request.Request.PhotoUrl,
                RequestId = request.RequestId,
                Type = ChannelTypeConvertor.Convert(request.Request.Type),
                WelcomeMessage = request.Request.WelcomeMessage
            };

            return await ValidateAndExecuteAsync(channelRequest, new CreateChannelRequestValidator(), new TaskReference<ChannelSummaryResponse>(async () =>
            {
                var createChannelRequest = new DomainRequest.Channel.CreateChannelRequest(Context.GetSaasUserId(), channelRequest.Name, channelRequest.Type)
                {
                    AllowedMembers = channelRequest.AllowedMembers,
                    Description = channelRequest.Description,
                    PhotoUrl = channelRequest.PhotoUrl,
                    WelcomeMessage = channelRequest.WelcomeMessage
                };
                return await _channelSocketService.CreateChannelAsync(createChannelRequest);
            }),
            request.RequestId);
        }

        public async Task<ChannelSummaryResponse> CreateDirectChannelAsync(SignalRRequest<Channel.CreateDirectChannelRequest> request)
        {
            var createRequest = new CreateDirectChannelRequest
            {
                MemberId = request.Request.MemberId,
                RequestId = request.RequestId
            };

            return await ValidateAndExecuteAsync(createRequest, new CreateDirectChannelRequestValidator(), new TaskReference<ChannelSummaryResponse>(async () =>
                {
                    var createChannelRequest =
                        new DomainRequest.Channel.CreateDirectChannelRequest(Context.GetSaasUserId(), createRequest.MemberId);

                    return await _channelSocketService.CreateDirectChannelAsync(createChannelRequest);
                }),
                request.RequestId);
        }

        public async Task<ChannelSummaryResponse> UpdateChannelAsync(SignalRRequest<Channel.UpdateChannelRequest> request)
        {
            var updateRequest = new UpdateChannelRequest
            {
                ChannelId = request.Request.ChannelId,
                Description = request.Request.Description,
                Name = request.Request.Name,
                PhotoUrl = request.Request.PhotoUrl,
                RequestId = request.RequestId,
                WelcomeMessage = request.Request.WelcomeMessage
            };

            return await ValidateAndExecuteAsync(updateRequest, new UpdateChannelRequestValidator(), new TaskReference<ChannelSummaryResponse>(async () =>
            {
                var updateChannelRequest = new DomainRequest.Channel.UpdateChannelRequest(Context.GetSaasUserId(), updateRequest.ChannelId, updateRequest.Name)
                {
                    PhotoUrl = request.Request.PhotoUrl,
                    Description = request.Request.Description,
                    WelcomeMessage = request.Request.WelcomeMessage
                };
                return await _channelSocketService.UpdateChannelAsync(updateChannelRequest);
            }),
            request.RequestId);
        }

        public async Task CloseChannelAsync(SignalRRequest<Channel.ChannelRequest> request)
        {
            var channelRequest = new ChannelRequest { ChannelId = request.Request.ChannelId, RequestId = request.RequestId };

            await ValidateAndExecuteAsync(channelRequest, new ChannelRequestValidator(), new TaskReference(async () =>
            {
                var closeChannelRequest = new DomainRequest.Channel.ChannelRequest(Context.GetSaasUserId(), channelRequest.ChannelId);
                await _channelSocketService.CloseChannelAsync(closeChannelRequest);
            }),
            request.RequestId);
        }

        public async Task LeaveChannelAsync(SignalRRequest<Channel.ChannelRequest> request)
        {
            var channelRequest = new ChannelRequest { ChannelId = request.Request.ChannelId, RequestId = request.RequestId };

            await ValidateAndExecuteAsync(channelRequest, new ChannelRequestValidator(), new TaskReference(async () =>
            {
                var closeChannelRequest = new DomainRequest.Channel.ChannelRequest(Context.GetSaasUserId(), channelRequest.ChannelId);
                await _channelSocketService.LeaveChannelAsync(closeChannelRequest);
            }),
            request.RequestId);
        }

        public async Task MuteChannelAsync(SignalRRequest<Channel.MuteChannelRequest> request)
        {
            var muteRequest = new MuteChannelRequest
            {
                ChannelId = request.Request.ChannelId,
                IsMuted = request.Request.IsMuted,
                RequestId = request.RequestId
            };

            await ValidateAndExecuteAsync(muteRequest, new MuteChannelRequestValidator(), new TaskReference(async () =>
            {
                await _channelService.MuteChannelAsync(Context.GetSaasUserId(), muteRequest.ChannelId, muteRequest.IsMuted);
            }),
            request.RequestId);
        }

        public async Task PinChannelAsync(SignalRRequest<Channel.PinChannelRequest> request)
        {
            var pinRequest = new PinChannelRequest
            {
                ChannelId = request.Request.ChannelId,
                IsPinned = request.Request.IsPinned,
                RequestId = request.RequestId
            };

            await ValidateAndExecuteAsync(pinRequest, new PinChannelRequestValidator(), new TaskReference(async () =>
            {
                await _channelService.PinChannelAsync(Context.GetSaasUserId(), pinRequest.ChannelId, pinRequest.IsPinned);
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
