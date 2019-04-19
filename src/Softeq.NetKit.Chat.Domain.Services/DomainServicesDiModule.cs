// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Chat.Domain.Services.Configuration;
using Softeq.NetKit.Chat.Domain.Services.DomainServices;
using Softeq.NetKit.Chat.Domain.Services.Mappings;
using Softeq.NetKit.Chat.Domain.Services.Utility;

namespace Softeq.NetKit.Chat.Domain.Services
{
    public class DomainServicesDiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x =>
                {
                    var context = x.Resolve<IComponentContext>();
                    var config = context.Resolve<IConfiguration>();
                    return new MessagesConfiguration(
                        Convert.ToInt32(config["Message:MessageAttachmentsLimit"]),
                        Convert.ToInt32(config["Message:LastMessageReadCount"]));
                })
                .AsSelf();

            builder.RegisterType<ChannelService>()
                .As<IChannelService>();

            builder.RegisterType<MemberService>()
                .As<IMemberService>();

            builder.RegisterType<MessageService>()
                .As<IMessageService>();
            
            builder.RegisterType<NotificationSettingsService>()
                .As<INotificationSettingsService>();

            builder.RegisterType<ChannelMemberService>()
                .As<IChannelMemberService>();

            builder.RegisterType<ClientService>()
                .As<IClientService>();

            builder.RegisterType<SystemDateTimeProvider>()
                .As<IDateTimeProvider>();

            RegisterMappings(builder);
        }

        private void RegisterMappings(ContainerBuilder builder)
        {
            builder.RegisterType<DomainModelsMapper>()
                .As<IDomainModelsMapper>();
        }
    }
}