// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Autofac;
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
            builder.RegisterType<MessagesConfiguration>()
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

            builder.RegisterType<DirectChannelService>()
                .As<IDirectChannelService>();

            builder.RegisterType<ChatSystemMessagesService>()
                .As<IChatSystemMessagesService>();

            RegisterMappings(builder);
        }

        private void RegisterMappings(ContainerBuilder builder)
        {
            builder.RegisterType<DomainModelsMapper>()
                .As<IDomainModelsMapper>();
        }
    }
}