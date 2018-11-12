// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Softeq.NetKit.Chat.Infrastructure.SignalR.Hubs
{
    public static class HubContextExtensions
    {
        public static string GetSaasUserId(this HubCallerContext callerContext)
        {
            return callerContext.User?.FindFirstValue("sub");
        }

        public static string GetUserName(this HubCallerContext callerContext)
        {
            return callerContext.User?.FindFirstValue("name");
        }
    }
}