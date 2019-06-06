// Developed by Softeq Development Corporation
// http://www.softeq.com

using FluentValidation.Results;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.SignalR.Hubs;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Channel;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Message;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Channel;
using Softeq.NetKit.Chat.Client.SDK.Models.CommonModels.Response.Message;

namespace Softeq.NetKit.Chat.Tests.Integration.ChatHub
{
    public class SignalRClient
    {
        private readonly string _serverBaseAddress;

        private HubConnection _connection;

        public SignalRClient(string serverBaseAddress)
        {
            _serverBaseAddress = serverBaseAddress;
        }

        public event Action ConnectionClosed;
        public event Action<ChannelSummaryResponse> ChannelCreated;
        public event Action<MemberSummaryResponse, ChannelSummaryResponse> MemberJoined;
        public event Action<IList<ValidationFailure>, string> ValidationFailed;
        public event Action<MessageResponse> MessageAdded;
        public event Action<MessageResponse> MessageUpdated;
        public event Action<Guid, ChannelSummaryResponse> MessageDeleted;
        public event Action<ChannelSummaryResponse> ChannelClosed;
        public event Action<ChannelSummaryResponse> ChannelUpdated;

        public async Task ConnectAsync(string accessToken, HttpMessageHandler handler)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{_serverBaseAddress}chat", options =>
                {
                    options.Transports = HttpTransportType.LongPolling;
                    options.HttpMessageHandlerFactory = _ => handler;
                    options.Headers.Add("Authorization", "Bearer " + accessToken);
                })
                .Build();

            _connection.Closed += e =>
            {
                ConnectionClosed?.Invoke();
                return Task.CompletedTask;
            };

            SubscribeToEvents();

            await _connection.StartAsync();
        }

        public async Task<ClientResponse> AddClientAsync()
        {
            return await _connection.InvokeAsync<ClientResponse>("AddClientAsync");
        }

        public async Task<ChannelSummaryResponse> CreateChannelAsync(CreateChannelRequest model)
        {
            return await _connection.InvokeAsync<ChannelSummaryResponse>("CreateChannelAsync", model);
        }

        public async Task<ChannelResponse> CloseChannelAsync(ChannelRequest model)
        {
            return await _connection.InvokeAsync<ChannelResponse>("CloseChannelAsync", model);
        }

        public async Task<ChannelResponse> UpdateChannelAsync(UpdateChannelRequest model)
        {
            return await _connection.InvokeAsync<ChannelResponse>("UpdateChannelAsync", model);
        }

        public async Task<MessageResponse> AddMessageAsync(AddMessageRequest model)
        {
            return await _connection.InvokeAsync<MessageResponse>("AddMessageAsync", model);
        }

        public async Task<MessageResponse> UpdateMessageAsync(UpdateMessageRequest model)
        {
            return await _connection.InvokeAsync<MessageResponse>("UpdateMessageAsync", model);
        }

        public async Task<MessageResponse> DeleteMessageAsync(DeleteMessageRequest model)
        {
            return await _connection.InvokeAsync<MessageResponse>("DeleteMessageAsync", model);
        }

        public async Task InviteMultipleMembersAsync(SignalR.TransportModels.Request.Member.InviteMultipleMembersRequest model)
        {
            await _connection.InvokeAsync("InviteMultipleMembersAsync", model);
        }

        public async Task LeaveChannelAsync(ChannelRequest model)
        {
            await _connection.InvokeAsync("LeaveChannelAsync", model);
        }

        private void SubscribeToEvents()
        {
            _connection.On<IList<ValidationFailure>, string>(HubEvents.RequestValidationFailed, (errors, requestId) => { Execute(ValidationFailed, action => action(errors, requestId)); });
            _connection.On<ChannelSummaryResponse>(HubEvents.ChannelCreated, channel => { Execute(ChannelCreated, action => action(channel)); });
            _connection.On<MemberSummaryResponse, ChannelSummaryResponse>(HubEvents.MemberJoined, (member, channel) => { Execute(MemberJoined, action => action(member, channel)); });
            _connection.On<MessageResponse>(HubEvents.MessageAdded, response => { Execute(MessageAdded, action => action(response)); });
            _connection.On<MessageResponse>(HubEvents.MessageUpdated, response => { Execute(MessageUpdated, action => action(response)); });
            _connection.On<Guid, ChannelSummaryResponse>(HubEvents.MessageDeleted, (id, response) => { Execute(MessageDeleted, action => action(id, response)); });
            _connection.On<ChannelSummaryResponse>(HubEvents.ChannelClosed, response => { Execute(ChannelClosed, action => action(response)); });
            _connection.On<ChannelSummaryResponse>(HubEvents.ChannelUpdated, response => { Execute(ChannelUpdated, action => action(response)); });
        }

        private void Execute<T>(T handlers, Action<T> action) where T : class
        {
            Task.Factory.StartNew(() =>
            {
                if (handlers != null)
                {
                    action(handlers);
                }
            });
        }
    }
}