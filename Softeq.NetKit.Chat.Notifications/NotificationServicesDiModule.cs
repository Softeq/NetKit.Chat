// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Autofac;
using Softeq.NetKit.Chat.Domain.Services.Configuration;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;
using Softeq.NetKit.Chat.Notifications.Services;

namespace Softeq.NetKit.Chat.Notifications
{
    public class NotificationServicesDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PushNotificationService>()
                .As<IPushNotificationService>();
        }
    }
}