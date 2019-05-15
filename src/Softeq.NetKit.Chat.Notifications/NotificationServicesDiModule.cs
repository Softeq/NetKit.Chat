// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Chat.Notifications.Services;
using Softeq.NetKit.Services.PushNotifications.Abstractions;
using Softeq.NetKit.Services.PushNotifications.Client;

namespace Softeq.NetKit.Chat.Notifications
{
    public class NotificationServicesDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PushNotificationService>()
                .As<IPushNotificationService>();

            builder.RegisterType<AzureNotificationHubSender>().As<IPushNotificationSender>();
            builder.RegisterType<AzureNotificationHubSubscriber>().As<IPushNotificationSubscriber>();

            builder.RegisterType<AzureNotificationHubSender>().As<IPushNotificationSender>();
            builder.RegisterType<AzureNotificationHubSubscriber>().As<IPushNotificationSubscriber>();

            builder.Register((context) =>
            {
                var configuration = context.Resolve<IConfiguration>();
                return new AzureNotificationHubConfiguration(
                    configuration["AzureNotificationHub:ConnectionString"],
                    configuration["AzureNotificationHub:HubName"]);
            }).SingleInstance();
        }
    }
}