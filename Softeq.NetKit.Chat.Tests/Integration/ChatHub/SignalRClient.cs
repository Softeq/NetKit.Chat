// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Channel;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Client;
using Softeq.NetKit.Chat.Domain.TransportModels.Response.Member;
using Softeq.NetKit.Chat.SignalR.Hubs;
using Softeq.NetKit.Chat.SignalR.TransportModels.Request.Channel;

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
        public event Action<MemberSummary, ChannelSummaryResponse> MemberJoined;
        public event Action<IList<ValidationFailure>, string> ValidationFailed;

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

        private void SubscribeToEvents()
        {
            _connection.On<IList<ValidationFailure>, string>(HubEvents.RequestValidationFailed, (errors, requestId) => { Execute(ValidationFailed, action => action(errors, requestId)); });
            _connection.On<ChannelSummaryResponse>(HubEvents.ChannelCreated, channel => { Execute(ChannelCreated, action => action(channel)); });
            _connection.On<MemberSummary, ChannelSummaryResponse>(HubEvents.MemberJoined, (member, channel) => { Execute(MemberJoined, action => action(member, channel)); });
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