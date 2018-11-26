// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Softeq.NetKit.Chat.Domain.Services;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Notifications;
using Softeq.NetKit.Chat.Notifications.PushNotifications;

namespace Softeq.NetKit.Chat.Tests.DI
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            #region Services

            builder.RegisterType<ChannelService>()
                .As<IChannelService>();

            builder.RegisterType<MemberService>()
                .As<IMemberService>();

            builder.RegisterType<MessageService>()
                .As<IMessageService>();

            builder.RegisterType<ChannelMemberService>()
                .As<IChannelMemberService>();

            #endregion
        }
    }
}