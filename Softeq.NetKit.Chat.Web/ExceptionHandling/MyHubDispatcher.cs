// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Internal;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Softeq.NetKit.Chat.SignalR.Hubs;

namespace Softeq.NetKit.Chat.Web.ExceptionHandling
{
    public class MyHubDispatcher : DefaultHubDispatcher<ChatHub>
    {
        public MyHubDispatcher(IServiceScopeFactory serviceScopeFactory, IHubContext<ChatHub> hubContext, IOptions<HubOptions<ChatHub>> hubOptions, IOptions<HubOptions> globalHubOptions, ILogger<DefaultHubDispatcher<ChatHub>> logger) 
            : base(serviceScopeFactory, hubContext, hubOptions, globalHubOptions, logger)
        {
        }

        public override Task OnConnectedAsync(HubConnectionContext connection)
        {
            return base.OnConnectedAsync(connection);
        }

        public override Task OnDisconnectedAsync(HubConnectionContext connection, Exception exception)
        {
            return base.OnDisconnectedAsync(connection, exception);
        }

        public override Task DispatchMessageAsync(HubConnectionContext connection, HubMessage hubMessage)
        {
            return base.DispatchMessageAsync(connection, hubMessage);
        }
    }
}