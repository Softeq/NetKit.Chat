// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Softeq.NetKit.Chat.Notifications.PushNotifications;

namespace Softeq.NetKit.Chat.Notifications
{
    public class DIModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            RegisterNotificationServices(builder);
        }

        private static void RegisterNotificationServices(ContainerBuilder builder)
        {
            builder.RegisterType<NotificationHub>().As<INotificationHub>();
        }
    }
}
