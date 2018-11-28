// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Softeq.NetKit.Chat.SignalR.Hubs.Notifications;
using Softeq.NetKit.Chat.SignalR.Sockets;

namespace Softeq.NetKit.Chat.SignalR
{
    public class SignalRDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterApplicationServices(builder);
            RegisterNotificationServices(builder);
        }

        private static void RegisterApplicationServices(ContainerBuilder builder)
        {
            builder.RegisterType<ChannelSocketService>().As<IChannelSocketService>();
            builder.RegisterType<MessageSocketService>().As<IMessageSocketService>();
        }

        private static void RegisterNotificationServices(ContainerBuilder builder)
        {
            builder.RegisterType<ChannelNotificationService>().As<IChannelNotificationService>();
            builder.RegisterType<MessageNotificationService>().As<IMessageNotificationService>();
        }
    }
}