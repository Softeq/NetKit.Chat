// // Developed by Softeq Development Corporation
// // http://www.softeq.com

using Autofac;
using Softeq.NetKit.Chat.Infrastructure.SignalR.Sockets;

namespace Softeq.NetKit.Chat.Infrastructure.SignalR
{
    public class DIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ChannelSocketService>()
                .As<IChannelSocketService>();

            builder.RegisterType<MessageSocketService>()
                .As<IMessageSocketService>();
        }
    }
}